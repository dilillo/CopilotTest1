using CopilotTest1.Core.Businesses;

namespace CopilotTest1.Core.Domain.Businesses
{
    [GenerateSerializer]
    public class BusinessState
    {
        [Id(0)]
        public BusinessProfile Profile { get; set; } = new BusinessProfile();

        [Id(1)]
        public List<Guid> Operators { get; set; } = new List<Guid>();

        [Id(2)]
        public bool IsPermanentlyClosed { get; set; } = false;

        public BusinessState Apply(BusinessOpenedEvent @event)
        {
            Profile = @event.Profile;
            Operators.Add(@event.OperatorId);

            return this;
        }   

        public BusinessState Apply(BusinessOperatorAddedEvent @event)
        {
            Operators.Add(@event.OperatorId);

            return this;
        }   

        public BusinessState Apply(BusinessOperatorRemovedEvent @event)
        {
            Operators.Remove(@event.OperatorId);

            return this;
        }   

        public BusinessState Apply(BusinessProfileModifiedEvent @event)
        {
            Profile = @event.Profile;

            return this;
        }

        public BusinessState Apply(BusinessPermanentlyClosedEvent @event)
        {
            IsPermanentlyClosed = true;

            return this;
        }
    }
}
