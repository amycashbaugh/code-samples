import axios from "axios";

const userEndpoint = "https://localhost:50001/api/users";

let login = (payload) => {

  const config = {
    method: "POST",
    url: userEndpoint + "/login",
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let register = (payload) => {

  const config = {
    method: "POST",
    url: userEndpoint + "/register",
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let currentUser = () => {

  const config = {
    method: "GET",
    url: userEndpoint + "/current",
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let logout = () => {

  const config = {
    method: "GET",
    url: userEndpoint + "/logout",
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let sendEmail = (payload) => {

  const config = {
    method: "POST",
    url: userEndpoint + "/emails",
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let emailConfirm = (token) => {

  const config = {
    method: "POST",
    url: userEndpoint + "/confirmation/" + token,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let userId = (id) =>{

  const config = {
    method: "GET",
    url: userEndpoint+ "/" + id,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};


let resetPassword = (token) =>{

  const config = {
    method: "PUT",
    url: userEndpoint+ "/passwordreset/" + token ,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let getByEmail = (email) => {

  const config = {
    method: "PUT",
    url: userEndpoint+ "/forgotpassword/" + email,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};


export { login, register, currentUser, logout, sendEmail, userId, resetPassword, emailConfirm, getByEmail }; 
