import axios from "axios";
import debug from "sabio-debug";

const _logger = debug.extend("User Service"); 

const userEndpoint = "https://localhost:50001/api/users";

let login = (payload) => {
    _logger("Login Service is firing........")

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
    _logger("Register Service is firing........")

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
  _logger("Current user Service is firing......")

  const config = {
    method: "GET",
    url: userEndpoint + "/current",
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let logout = () => {
  _logger("Logout Service is firing......")

  const config = {
    method: "GET",
    url: userEndpoint + "/logout",
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let sendEmail = (payload) => {
    _logger("Email Service is firing........")

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
    _logger("Confirming email...")

  const config = {
    method: "POST",
    url: userEndpoint + "/confirmation/" + token,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let userId = (id) =>{
    _logger("Getting User By Id is firing........")

  const config = {
    method: "GET",
    url: userEndpoint+ "/" + id,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};


let resetPassword = (token) =>{
    _logger("password reset is firing........")

  const config = {
    method: "PUT",
    url: userEndpoint+ "/passwordreset/" + token ,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

let getByEmail = (email) => {
    _logger("password reset is firing........")

  const config = {
    method: "PUT",
    url: userEndpoint+ "/forgotpassword/" + email,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};


export { login, register, currentUser, logout, sendEmail, userId, resetPassword, emailConfirm, getByEmail }; // export all your calls here

// if you had three functions to export 
// export { logIn, register, thirdFunction }