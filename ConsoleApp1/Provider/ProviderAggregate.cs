using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Infrastructure;
using Orleans.EventSourcing;

namespace ConsoleApp1.Provider
{
    public class ProviderAggregate : JournaledGrain<ProviderState>
    {
        public async Task Register(string name)
        {
            RaiseEvent(new ProviderRegisteredEvent { Name = name });

            await ConfirmEvents();
        }

        public async Task ModifyName(string name)
        {
            RaiseEvent(new ProviderNameModifiedEvent { Name = name });

            await ConfirmEvents();
        }
    }
}
