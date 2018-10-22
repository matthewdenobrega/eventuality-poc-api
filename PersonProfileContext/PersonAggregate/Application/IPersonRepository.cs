using EventualityPOC.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using System.Threading.Tasks;

namespace EventualityPOC.PersonProfileContext.PersonAggregate.Application
{
    public interface IPersonRepository
    {
        Task InitializeAsync();
        Person RetrievePerson(string id);
        Task SaveDecisionAsync(StatementExtension statement);
        Task SavePerceptionAsync(StatementExtension statement);
    }
}
