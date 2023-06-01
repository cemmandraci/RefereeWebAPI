using RefereeApp.Models.RefLevels;

namespace RefereeApp.Abstractions;

public interface IRefereeLevelService
{
    Task<List<RefereeLevelsResponseModel>> Get();
    Task<RefereeLevelsResponseModel> Create(CreateRefereeLevelsRequestModel request);
    Task<RefereeLevelsResponseModel> Update(UpdateRefereeLevelsRequestModel request);
}