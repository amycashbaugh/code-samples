using Sabio.Models.Domain.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain
{
    public class ListingDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int DaysAvailable { get; set; }
        public bool HasReservation { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int StatusId { get; set; }
        public KitchenProfile Kitchen { get; set; }
        public List<LookUp> RentalTypes { get; set; }
        public List<EntityCost> ListingServices { get; set; }
        public List<EntityCost> ListingCosts { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateModifed { get; set; }
    }
}
