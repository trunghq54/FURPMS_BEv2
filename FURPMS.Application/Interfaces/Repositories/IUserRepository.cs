using FURPMS.Domain.Entities.MasterData;
using FURPMS.Domain.Entities.Users;

namespace FURPMS.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    IQueryable<Role> Roles { get; }
    IQueryable<UserRole> UserRoles { get; }
    IQueryable<AcademicProfile> AcademicProfiles { get; }
    IQueryable<OrganizationalUnit> OrganizationalUnits { get; }
    void AddUserRole(UserRole userRole);
    void AddAcademicProfile(AcademicProfile profile);
    void RemoveUserRoles(IEnumerable<UserRole> userRoles);

    /// <summary>Lĩnh vực chuyên môn đã khai — nguồn để xét "đúng chuyên môn" khi lập hội đồng.</summary>
    IQueryable<UserResearchTrack> UserResearchTracks { get; }
    IQueryable<ResearchTrack> ResearchTracks { get; }
    void AddUserResearchTrack(UserResearchTrack track);
    void RemoveUserResearchTracks(IEnumerable<UserResearchTrack> tracks);
}
