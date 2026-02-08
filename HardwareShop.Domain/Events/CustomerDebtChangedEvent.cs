
using HardwareShop.Domain.Abstracts;

namespace HardwareShop.Domain.Events
{
    public class CustomerDebtChangedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public double OldDebt { get; }
        public double ChangeOfDebt { get; }

        public CustomerDebtChangedEvent(Guid customerId, double oldDebt, double changeOfDebt)
        {
            CustomerId = customerId;
            OldDebt = oldDebt;
            ChangeOfDebt = changeOfDebt;
        }
    }
}