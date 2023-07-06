using RefereeApp.Models.ClubModels;

namespace RefereeApp.Abstractions;

public interface IClubService
{
    Task<ClubResponseModel> Get(int id);
    Task<List<ClubResponseModel>> GetAll();
    Task<ClubResponseModel> Create(CreateClubRequestModel request);
    Task<ClubResponseModel> Update(UpdateClubRequestModel request);
}