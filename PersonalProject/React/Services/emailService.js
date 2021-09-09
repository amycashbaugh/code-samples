import axios from "axios";

let email = (payload) => {
    console.log("Email Service is firing........")

  const config = {
    method: "POST",
    url: "https://api.remotebootcamp.dev/api/emails",
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };

  return axios(config);
};

export {email}