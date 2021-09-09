import axios from "axios";

const eventEndpoint = "https://localhost:50001/api/events";

let add = (payload) => {
  const config = {
    method: "POST",
    url: eventEndpoint,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config);
};

let getPage = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: eventEndpoint + "/paginate?pageIndex=" + pageIndex + "&pageSize=" + pageSize,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config);
};

let update = (id, payload) => {
    const config = {
      method: "PUT",
      url: eventEndpoint + "/" + id,
      data: payload,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
  
    return axios(config)
  
};

let search = (pageIndex, pageSize,dateStart, dateEnd) => {
    const config = {
      method: "GET",
      url: eventEndpoint + "/search?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&dateStart=" + dateStart + "&dateEnd=" + dateEnd,
      crossdomain: true,
      headers: { "Content-Type": "application/json" },
    };
  
    return axios(config)
  
};
  


export { add, getPage, update, search };
