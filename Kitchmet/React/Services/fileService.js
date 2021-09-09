import axios from "axios";
import * as helper from "./serviceHelpers";

const endpoint = `${helper.API_HOST_PREFIX}/api/files`;

let add = (files) => {
    _logger("Adding Files.....");

    const config = {
        method: "POST",
        url: endpoint+ "/upload",
        data: files,
        crossdomain: true,
        headers: { "Content-Type": "application/json" },
    };

    return axios(config);
};

export { add };
