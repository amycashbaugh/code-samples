//Work in progress
import React, { Fragment, useState } from "react";
import SplashNavBar from "../components/splash/SplashNavBar";
import { Button, Grid } from "@material-ui/core";
import chefs from "@assets/images/kitchmet/33.png";
import LocationAutoComplete from "../components/forms/LocationAutoComplete";
import SplashReviews from "../components/splash/SplashReviews";
import SplashBody from "../components/splash/SplashBody";
import SplashFooter from "../components/splash/SplashFooter";
import logger from "sabio-debug";
import PropTypes from "prop-types";

const _logger = logger.extend("SplashPage");

const SplashPage = (props) => {
  const [coordinates, setCoordinates] = useState(null);

  const handlePlaceChanged = (location) => {
    _logger(location);
    setCoordinates({ lat: location.latitude, lng: location.longitude });
  };

  const searchListingByLocation = () => {
    props.history.push("/listings", { type: "LOC_DATA", payload: coordinates });
  };

  return (
    <Fragment>
      <div className="hero-wrapper">
        <div className="flex-grow-5 d-flex align-items-center">
          <div
            className="bg-composed-wrapper--image bg-composed-filter-rm opacity-9 h-100"
            style={{ backgroundImage: "url(" + chefs + ")" }}
          />
          <div className="bg-composed-wrapper--content justify-content-center">
            <SplashNavBar {...props} />
            <Grid className="container text-center my-5 py-5 justify-content-center align-items-center">
              <Grid
                item
                className="d-flex text-center justify-content-center align-items-center"
              >
                <div
                  className="text-white justify-content-center"
                  style={{
                    paddingTop: "30px",
                  }}
                >
                  <h1
                    className="display-1 font-weight-bold my-4 pt-5 mt-5"
                    style={{
                      textShadow: "0px 10px 10px #737373",
                      fontFamily: "Helvetica",
                    }}
                  >
                    Find your ideal kitchen
                  </h1>
                  <div className=" justify-content-center w-99">
                    <LocationAutoComplete
                      handleChange={handlePlaceChanged}
                      placeholder="Enter location here..."
                    />
                    <Button
                      color="inherit"
                      variant="contained"
                      className="m-3 text-white btn-gradient bg-plum-plate"
                      onClick={searchListingByLocation}
                    >
                      Search
                    </Button>
                  </div>
                </div>
              </Grid>
            </Grid>
          </div>
        </div>
      </div>
      <div className="pb-5 my-3">
        <SplashBody currentUser={props.currentUser} />
      </div>
      <div>
        <SplashReviews />
      </div>
      <div className="py-5 bg-midnight-bloom">
        <div>
          <SplashFooter {...props} />
          <div className="mt-5">
            <span className="text-center d-block text-white-50">Kitchmet</span>
          </div>
        </div>
      </div>
    </Fragment>
  );
};

SplashPage.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func,
  }),
};

SplashPage.propTypes = {
  currentUser: PropTypes.shape({
    roles: PropTypes.arrayOf(PropTypes.string),
    userName: PropTypes.string.isRequired,
    email: PropTypes.string.isRequired,
  }),
};

export default SplashPage;
