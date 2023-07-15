using RefereeApp.Models.FixtureModels;

namespace RefereeApp.Abstractions;

public interface IFixtureService
{
    Task<FixtureResponseModel> GetById(int id);
    Task<List<FixtureResponseModel>> Get();
    Task<FixtureResponseModel> Create(CreateFixtureRequestModel request);
    Task<FixtureResponseModel> Update(UpdateFixtureRequestModel request);
}