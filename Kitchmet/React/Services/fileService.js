import axios from "axios";
import debug from "sabio-debug";
import * as helper from "./serviceHelpers";

const _logger = debug.extend("File");

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