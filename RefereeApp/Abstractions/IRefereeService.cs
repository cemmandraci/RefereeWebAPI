using RefereeApp.Models.RefereeModels;

namespace RefereeApp.Abstractions;

public interface IRefereeService
{
    Task<List<RefereeResponseModel>> Get();
    Task<RefereeResponseModel> GetById(int id);
    Task<RefereeResponseModel> Create(CreateRefereeRequestModel request);
    Task<RefereeResponseModel> Update(UpdateRefereeRequestModel request);
}