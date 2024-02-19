using CopilotTest1.Core.Businesses;
using CopilotTest1.Core.Data;
using CopilotTest1.Core.Data.Entities;
using CopilotTest1.Core.Infrastructure;
using Orleans;
using Sketch.EventSourcing;

namespace CopilotTest1.Core.Domain.Businesses
{
    public interface IBusinessAggregate : IGrainWithGuidKey
    {
        Task<BusinessState> GetState();

        Task Open(Guid operatorId, BusinessProfile profile);

        Task ModifyProfile(BusinessProfile profile);

        Task AddOperator(Guid operatorId);

        Task RemoveOperator(Guid operatorId);

        Task PermanentlyClose();
    }

    public class BusinessAggregate : Aggregate<BusinessState, CoreDbContext>, IBusinessAggregate
    {
        public BusinessAggregate(CoreDbContext dbContext) : base(dbContext)
        {
        }

        public Task<BusinessState> GetState() => Task.FromResult(State);

        public async Task Open(Guid operatorId, BusinessProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var existingBusiness = await DbContext.GetBusinessRefByName(profile.Name);

            if (existingBusiness != null)
                throw new DomainException("Business name already exists.");

            RaiseDomainEvent<BusinessOpenedEvent>((e) =>
            {
                e.OperatorId = operatorId;
                e.Profile = profile;
            });

            await ConfirmEvents();
        }

        public async Task AddOperator(Guid operatorId)
        {
            RaiseDomainEvent<BusinessOperatorAddedEvent>((e) =>
            {
                e.OperatorId = operatorId;
            });

            await ConfirmEvents();
        }

        public async Task RemoveOperator(Guid operatorId)
        {
            if (State.Operators.All(i => i != operatorId))
                return;

            RaiseDomainEvent<BusinessOperatorRemovedEvent>((e) =>
            {
                e.OperatorId = operatorId;
            });

            await ConfirmEvents();
        }

        public async Task ModifyProfile(BusinessProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var existingBusiness = await DbContext.GetBusinessRefByName(profile.Name);

            if (existingBusiness != null && existingBusiness.Id != this.GetPrimaryKey())
                throw new DomainException("Business name already exists.");

            RaiseDomainEvent<BusinessProfileModifiedEvent>((e) =>
            {
                e.Profile = profile;
            });

            await ConfirmEvents();
        }

        public async Task PermanentlyClose()
        {
            if (State.IsPermanentlyClosed)
                return;

            RaiseDomainEvent<BusinessPermanentlyClosedEvent>();

            await ConfirmEvents();
        }

        protected override async Task ApplyingUpdatesToStorage(IReadOnlyList<Event> updates)
        {
            var existingRef = await DbContext.GetBusinessRef(this.GetPrimaryKey());

            if (existingRef == null)
            {
                existingRef = new BusinessRef
                {
                    Id = this.GetPrimaryKey(),
                };

                DbContext.BusinessRefs.Add(existingRef);
            }

            foreach (var item in updates)
            {
                switch (item)
                {
                    case BusinessOpenedEvent businessOpenedEvent:

                        existingRef.Name = businessOpenedEvent.Profile?.Name ?? string.Empty;

                        break;

                    case BusinessProfileModifiedEvent businessProfileModifiedEvent:

                        existingRef.Name = businessProfileModifiedEvent.Profile?.Name ?? string.Empty;

                        break;

                    case BusinessPermanentlyClosedEvent:

                        existingRef.IsPermanentlyClosed = true;

                        break;
                }
            }
        }
    }
}
