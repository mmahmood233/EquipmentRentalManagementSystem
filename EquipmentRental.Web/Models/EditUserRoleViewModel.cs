using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Web.Models.Admin
{
    public class EditUserRoleViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int SelectedRoleId { get; set; }
        public List<UserRole> AvailableRoles { get; set; }
    }
}
