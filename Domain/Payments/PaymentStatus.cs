using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payments
{
    public enum PaymentStatus
    {
        ToPay = 0,
        Paid = 1,
        Overdue = 2
    }
}
