using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Listings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sabio.Services.Interfaces
{
    public interface IListingsService
    {
        Listing GetById(int id);

        Paged<Listing> GetCurrentPage(int pageIndex, int pageSize, int userId);

        Paged<Listing> GetPage(int pageIndex, int pageSize);

        int Create(ListingAddRequest model, int userId);

        void Update(ListingUpdateRequest model);

        void UpdateIsActiveById(bool isActive, int listingId);

        public Paged<ListingDetails> SearchByLocation(int pageIndex, int pageSize, double lat, double lng, int distance);

        int Create(ListingDetailAddRequest model, int userId);

        Paged<ListingDetails> SelectAll(int pageIndex, int pageSize);

        Paged<ListingDetails> SelectByCreatedBy(int createdBy, int pageIndex, int pageSize);

        ListingDetails SelectById(int id);

        void Update(ListingDetailUpdateRequest model);

        public void UpdateStatus(int id);
    }
}
