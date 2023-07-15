using RefereeApp.Models.ClubModels;

namespace RefereeApp.Abstractions;

public interface IClubService
{
    Task<ClubResponseModel> GetById(int id);
    Task<List<ClubResponseModel>> Get();
    Task<ClubResponseModel> Create(CreateClubRequestModel request);
    Task<ClubResponseModel> Update(UpdateClubRequestModel request);
}