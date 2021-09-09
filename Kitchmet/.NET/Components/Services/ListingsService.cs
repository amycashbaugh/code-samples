using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Listings;
using Sabio.Models.Requests.Listings;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sabio.Services
{
    public class ListingsService : IListingsService
    {
        IDataProvider _dataProvider = null;
        IKitchenProfileService _kitchenMapper = null;

        public ListingsService(IDataProvider data, IKitchenProfileService kitchenMapper)
        {
            _dataProvider = data;
            _kitchenMapper = kitchenMapper;
        }

        public Listing GetById(int id)
        {
            string procName = "[dbo].[Listings_SelectById]";
            Listing listing = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {
                parameterCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int idx = 0;
                listing = MapListing(reader, ref idx);

            });
            return listing;
        }

        public Paged<Listing> GetCurrentPage(int pageIndex, int pageSize, int userId)
        {
            Paged<Listing> pagedList = null;
            List<Listing> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Listings_SelectByCreatedByPaginated]";

            _dataProvider.ExecuteCmd(procName,
                (param) =>
                {
                    param.AddWithValue("@pageIndex", pageIndex);
                    param.AddWithValue("@pageSize", pageSize);
                    param.AddWithValue("@CreatedBy", userId);
                },
                (reader, recordSetIndex) =>
                {
                    int idx = 0;
                    Listing listing = MapListing(reader, ref idx);


                    if (list == null)
                    {
                        list = new List<Listing>();
                    }
                    list.Add(listing);
                }
            );

            if (list != null)
            {
                pagedList = new Paged<Listing>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Paged<Listing> GetPage(int pageIndex, int pageSize)
        {
            Paged<Listing> pagedList = null;
            List<Listing> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Listings_SelectPaginated]";

            _dataProvider.ExecuteCmd(procName,
                (param) =>
                {
                    param.AddWithValue("@pageIndex", pageIndex);
                    param.AddWithValue("@pageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int idx = 0;
                    Listing listing = MapListing(reader, ref idx);


                    if (list == null)
                    {
                        list = new List<Listing>();
                    }
                    list.Add(listing);
                }
            );

            if (list != null)
            {
                pagedList = new Paged<Listing>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public int Create(ListingAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Listings_Insert]";

            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonListingParams(model, col);
                    col.AddWithValue("@CreatedBy", userId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);

                }, returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);
                }
            );
            return id;
        }

        public void Update(ListingUpdateRequest model)
        {
            string procName = "[dbo].[Listings_Update]";

            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonListingParams(model, col);
                    col.AddWithValue("@Id", model.Id);

                }, returnParameters: null
            );
        }

        public void UpdateIsActiveById(bool isActive, int listingId)
        {
            string procName = "[dbo].[Listings_UpdateIsActiveById]";

            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@IsActive", isActive);
                    col.AddWithValue("@Id", listingId);
                }, returnParameters: null
            );
        }

        public int Create(ListingDetailAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[ListingWizard_Insert]";
            DataTable rentalTypes = null;
            DataTable listingCosts = null;
            DataTable listingServices = null;

            if (model.RentalTypes != null)
            {
                rentalTypes = MapRentalTypesToTable(model.RentalTypes);
            }

            if (model.ListingCosts != null)
            {
                listingCosts = MapListing(model.ListingCosts, EntityType.ListingCost);
            }
            if (model.ListingServices != null)
            {
                listingServices = MapListing(model.ListingServices, EntityType.KitchenService);
            }

            _dataProvider.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(model, collection);
                    collection.AddWithValue("@CreatedBy", userId);
                    collection.AddWithValue("@BatchRentalTypes", rentalTypes);
                    collection.AddWithValue("@BatchListingCosts", listingCosts);
                    collection.AddWithValue("@BatchListingServices", listingServices);


                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    collection.Add(idOut);

                }, returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);
                }
            );
            return id;
        }

        public void Update(ListingDetailUpdateRequest model)
        {
            string procName = "[dbo].[ListingWizard_Update]";
            DataTable rentalTypes = null;
            DataTable listingCosts = null;
            DataTable listingServices = null;

            if (model.RentalTypes != null)
            {
                rentalTypes = MapRentalTypesToTable(model.RentalTypes);
            }
            if (model.ListingCosts != null)
            {
                listingCosts = MapListing(model.ListingCosts, EntityType.ListingCost);
            }
            if (model.ListingServices != null)
            {
                listingServices = MapListing(model.ListingServices, EntityType.KitchenService);
            }

            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(model, collection);
                collection.AddWithValue("@Id", model.Id);
                collection.AddWithValue("@BatchRentalTypes", rentalTypes);
                collection.AddWithValue("@BatchListingCosts", listingCosts);
                collection.AddWithValue("@BatchListingServices", listingServices);


            }, returnParameters: null
            );
        }

        public ListingDetails SelectById(int id)
        {
            string procName = "dbo.ListingWizard_SelectById";
            ListingDetails singleListingWizard = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int idx = 0;
                singleListingWizard = MapListingWizard(reader, ref idx);
            }
            );

            return singleListingWizard;
        }

        public Paged<ListingDetails> SelectAll(int pageIndex, int pageSize)
        {
            Paged<ListingDetails> pagedList = null;
            List<ListingDetails> list = null;
            int totalCount = 0;
            string procName = "dbo.ListingWizard_SelectAll";


            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);

            }, (reader, recordSetIndex) =>
            {
                int idx = 0;
                ListingDetails singleListing = MapListingWizard(reader, ref idx);

                totalCount = reader.GetSafeInt32(idx);

                if (list == null)
                {
                    list = new List<ListingDetails>();
                }
                list.Add(singleListing);
            }
                );

            if (list != null)
            {
                pagedList = new Paged<ListingDetails>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Paged<ListingDetails> SelectByCreatedBy(int createdBy, int pageIndex, int pageSize)
        {
            Paged<ListingDetails> pagedList = null;
            List<ListingDetails> list = null;
            int totalCount = 0;
            string procName = "[dbo].[ListingWizard_SelectByCreatedBy]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CreatedBy", createdBy);
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            },
                (reader, recordSetIndex) =>
                {
                    int idx = 0;
                    ListingDetails singleListing = MapListingWizard(reader, ref idx);

                    totalCount = reader.GetSafeInt32(idx);

                    if (list == null)
                    {
                        list = new List<ListingDetails>();
                    }
                    list.Add(singleListing);
                }
            );

            if (list != null)
            {
                pagedList = new Paged<ListingDetails>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Paged<ListingDetails> SearchByLocation(int pageIndex, int pageSize, double lat, double lng, int distance)
        {
            Paged<ListingDetails> pagedList = null;
            List<ListingDetails> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Listings_SelectByLocation]";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@pageIndex", pageIndex);
                paramCollection.AddWithValue("@pageSize", pageSize);
                paramCollection.AddWithValue("@lat", lat);
                paramCollection.AddWithValue("@lng", lng);
                paramCollection.AddWithValue("@distance", distance);
            },
               (reader, recordSetIndex) =>
               {
                   int idx = 0;
                   ListingDetails singleListing = MapListingWizard(reader, ref idx);

                   if (totalCount == 0)
                   {
                       totalCount = reader.GetSafeInt32(idx++);
                   }
                   if (list == null)
                   {
                       list = new List<ListingDetails>();
                   }
                   list.Add(singleListing);
               });

            if (list != null)
            {
                pagedList = new Paged<ListingDetails>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }



        public void UpdateStatus(int id)
        {
            string procName = "[dbo].[ListingWizard_DeleteById]";
            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            },
            returnParameters: null
            );
        }
        private DataTable MapRentalTypesToTable(List<int> mapRentalTypes)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            foreach (var rentalType in mapRentalTypes)
            {
                DataRow dataRow = dataTable.NewRow();
                int startingIdx = 0;
                dataRow.SetField(startingIdx++, rentalType);

                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        private DataTable MapListing(List<EntityCost> mapListingServices, EntityType entityType)
        {
            DataTable dataTable = new DataTable();

            if (entityType == EntityType.KitchenService)
            {
                dataTable.Columns.Add("ServiceTypeId", typeof(int));
            }
            dataTable.Columns.Add("Cost", typeof(decimal));
            dataTable.Columns.Add("CostTypeId", typeof(int));

            foreach (var listingService in mapListingServices)
            {
                DataRow dataRow = dataTable.NewRow();
                if (entityType == EntityType.KitchenService)
                {
                    dataRow.SetField("ServiceTypeId", listingService.EntityTypeId);
                }
                dataRow.SetField("Cost", listingService.Cost);
                dataRow.SetField("CostTypeId", listingService.CostTypeId);

                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        private Listing MapListing(IDataReader reader, ref int startingIndex)
        {
            Listing listing = new Listing();

            listing.Id = reader.GetSafeInt32(startingIndex++);
            listing.Name = reader.GetString(startingIndex++);
            listing.Title = reader.GetSafeString(startingIndex++);
            listing.ShortDescription = reader.GetSafeString(startingIndex++);
            listing.Description = reader.GetString(startingIndex++);
            listing.RentalTypes = reader.DeserializeObject<List<LookUp>>(startingIndex++);
            listing.CheckInTime = reader.GetSafeDateTime(startingIndex++);
            listing.CheckOutTime = reader.GetSafeDateTime(startingIndex++);
            listing.DaysAvailable = reader.GetSafeInt32(startingIndex++);
            listing.HasReservation = reader.GetBoolean(startingIndex++);
            listing.IsActive = reader.GetSafeBool(startingIndex++);
            listing.CreatedBy = reader.GetSafeInt32(startingIndex++);
            listing.DateCreated = reader.GetSafeDateTime(startingIndex++);
            listing.DateModifed = reader.GetSafeDateTime(startingIndex++);
            listing.Kitchen = _kitchenMapper.MapKitchen(reader, ref startingIndex);

            return listing;
        }

        private void AddCommonListingParams(ListingAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Name", model.Name);
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@ShortDescription", model.ShortDescription);
            col.AddWithValue("@Description", model.Description);
            col.AddWithValue("@KitchenProfileId", model.KitchenProfileId);
            col.AddWithValue("@CheckInTime", model.CheckInTime);
            col.AddWithValue("@CheckOutTime", model.CheckOutTime);
            col.AddWithValue("@DaysAvailable", model.DaysAvailable);
        }



        private ListingDetails MapListingWizard(IDataReader reader, ref int startingIndex)
        {
            ListingDetails listingDetails = new ListingDetails();

            listingDetails.Id = reader.GetSafeInt32(startingIndex++);
            listingDetails.Name = reader.GetString(startingIndex++);
            listingDetails.Title = reader.GetSafeString(startingIndex++);
            listingDetails.ShortDescription = reader.GetSafeString(startingIndex++);
            listingDetails.Description = reader.GetString(startingIndex++);
            listingDetails.CheckInTime = reader.GetSafeDateTime(startingIndex++);
            listingDetails.CheckOutTime = reader.GetSafeDateTime(startingIndex++);
            listingDetails.DaysAvailable = reader.GetSafeInt32(startingIndex++);
            listingDetails.HasReservation = reader.GetBoolean(startingIndex++);
            listingDetails.IsActive = reader.GetSafeBool(startingIndex++);
            listingDetails.CreatedBy = reader.GetSafeInt32(startingIndex++);
            listingDetails.StatusId = reader.GetSafeInt32(startingIndex++);
            listingDetails.RentalTypes = reader.DeserializeObject<List<LookUp>>(startingIndex++);
            listingDetails.ListingServices = reader.DeserializeObject<List<EntityCost>>(startingIndex++);
            listingDetails.ListingCosts = reader.DeserializeObject<List<EntityCost>>(startingIndex++);
            listingDetails.DateCreated = reader.GetSafeDateTime(startingIndex++);
            listingDetails.DateModifed = reader.GetSafeDateTime(startingIndex++);
            listingDetails.Kitchen = _kitchenMapper.MapKitchen(reader, ref startingIndex);


            return listingDetails;
        }

        private void AddCommonParams(ListingDetailAddRequest model, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Name", model.Name);
            collection.AddWithValue("@Title", model.Title);
            collection.AddWithValue("@ShortDescription", model.ShortDescription);
            collection.AddWithValue("@Description", model.Description);
            collection.AddWithValue("@KitchenProfileId", model.KitchenProfileId);
            collection.AddWithValue("@CheckInTime", model.CheckInTime);
            collection.AddWithValue("@CheckOutTime", model.CheckOutTime);
            collection.AddWithValue("@DaysAvailable", model.DaysAvailable);

        }
    }
}
