import axios from "axios";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";
import logger from "sabio-debug";

const _logger = logger.extend("listingService");

const endpoint = `${API_HOST_PREFIX}/api/listings`;

let selectListingById = (id) => {
  _logger("getting listing id");
  const config = {
    method: "GET",
    url: endpoint + "/" + id,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let getListings = (pageIndex, pageSize) => {
  _logger("getting all listings");
  const config = {
    method: "GET",
    url: `${endpoint}/paginate?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let getByCreator = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: endpoint + `/currrent/?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let getListingsByLocation = (pageIndex, pageSize, lat, lng, distance) => {
  _logger("getting all listings");
  const config = {
    method: "GET",
    url: `${endpoint}/searchlocation?pageIndex=${pageIndex}&pageSize=${pageSize}&lat=${lat}&lng=${lng}&distance=${distance}`,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};


export { selectListingById, getListings, getByCreator, getListingsByLocation };
