using RefereeApp.Models.FixtureModels;

namespace RefereeApp.Abstractions;

public interface IFixtureService
{
    Task<FixtureResponseModel> Get(int id);
    Task<List<FixtureResponseModel>> GetAll();
    Task<FixtureResponseModel> Create(CreateFixtureRequestModel request);
    Task<FixtureResponseModel> Update(UpdateFixtureRequestModel request);
}