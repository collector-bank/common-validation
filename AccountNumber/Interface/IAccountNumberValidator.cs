using System.Threading.Tasks;
using Collector.Common.Validation.AccountNumber.Models;

namespace Collector.Common.Validation.AccountNumber.Interface
{
    public interface IAccountNumberValidator
    {
        BankAccountModel  Identify(string number);
        bool              TryIdentify(string number, out BankAccountModel result);
        bool              TryValidate(string number, out BankAccountModel result);
        BankAccountModel  Validate(string number);
    }
}