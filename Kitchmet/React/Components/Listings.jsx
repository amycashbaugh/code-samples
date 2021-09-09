import React, { Fragment, useEffect, useState } from "react";
import { WrapperSimple } from "../layout-components";
import { Grid, Button } from "@material-ui/core";
import PropTypes from "prop-types";
import ListingCard from "../components/listings/ListingCard";
import * as listingService from "../services/listingService";
import LocationAutoComplete from "../components/forms/LocationAutoComplete";
import Pagination from "@material-ui/lab/Pagination";
import logger from "sabio-debug";

const _logger = logger.extend("Listings");

const Listings = (props) => {
  const [listing, setListing] = useState();
  const [page, setPage] = useState(1);
  const [pageSize] = useState(6);
  const [totalPages, setTotalPages] = useState();
  const [coordinates, setCoordinates] = useState();
  const [searchChange, setSearchChange] = useState(false);

  useEffect(() => {
    if (coordinates) {
      let lat = coordinates.lat;
      let lng = coordinates.lng;
      listingService
        .getListingsByLocation(page - 1, pageSize, lat, lng, 50)
        .then(onGetListingsSuccess)
        .catch(onGetListingsError);
    } else if (props.location.state) {
      let lat = props.location.state.payload.lat;
      let lng = props.location.state.payload.lng;
      listingService
        .getListingsByLocation(page - 1, pageSize, lat, lng, 50)
        .then(onGetListingsSuccess)
        .catch(onGetListingsError);
    } else {
      listingService
        .getListings(page - 1, pageSize)
        .then(onGetListingsSuccess)
        .catch(onGetListingsError);
    }
  }, [page, searchChange]);

  const mapListings = (listing) => {
    return (
      <ListingCard key={listing.id} listing={listing} viewMore={viewMore} />
    );
  };

  const handlePlaceChanged = (location) => {
    setCoordinates({ lat: location.latitude, lng: location.longitude });
  };

  const searchByLocation = () => {
    _logger(coordinates);
    setSearchChange(!searchChange);
  };

  const viewMore = (listing) => {
    props.history.push(`/listingdetails/${listing.id}`, {
      type: "listing_data",
      payload: listing,
    });
  };

  const onPageChange = (event, newPage) => {
    setPage(newPage);
  };

  const onGetListingsSuccess = (response) => {
    _logger(response);
    let mappedListings = response.item.pagedItems;
    setTotalPages(response.item.totalPages);
    setListing(mappedListings.map(mapListings));
  };

  const onGetListingsError = (error) => {
    _logger(error);
  };

  return (
    <Fragment>
      <WrapperSimple sectionHeading="Listings Near You">
        <Grid container spacing={2} className="justify-content-center">
          <Grid item xs={4}>
            <LocationAutoComplete
              handleChange={handlePlaceChanged}
              label="Location"
              placeholder="Search by location..."
            />
          </Grid>
          <Grid item xs={2}>
            <Button
              type="submit"
              className="btn-gradient bg-plum-plate text-white mt-3 justify-content-center"
              variant="contained"
              size="large"
              onClick={searchByLocation}
            >
              Find Listings
            </Button>
          </Grid>
        </Grid>
        <Grid
          container
          spacing={4}
          className="justify-content-center px-4 pt-5"
        >
          {listing}
        </Grid>
        <Grid container className="mt-5 justify-content-center mb-5 mt-5 pt-5">
          <Grid item>
            <Pagination
              count={totalPages}
              page={page}
              variant="outlined"
              shape="rounded"
              onChange={onPageChange}
            />
          </Grid>
        </Grid>
      </WrapperSimple>
    </Fragment>
  );
};

Listings.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func,
  }),
  location: PropTypes.shape({
    state: PropTypes.shape({
      payload: PropTypes.shape({
        lat: PropTypes.number,
        lng: PropTypes.number,
      }),
      type: PropTypes.string,
    }),
  }),
};

export default Listings;
