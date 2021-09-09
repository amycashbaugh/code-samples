import React, { Fragment, useState } from "react";
import PropTypes from "prop-types";
import { Grid } from "@material-ui/core";
import CloudUploadIcon from "@material-ui/icons/CloudUpload";
import Dropzone from "react-dropzone";
import * as fileService from "../../services/fileService";
import MuiAlert from "@material-ui/lab/Alert";


const FileUploadDropzone = (props) => {
  const [failed, setFailed] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [isSuccessful, setIsSuccessful] = useState(false);

  const onDrop = (files) => {
    const formData = new FormData();
    setIsUploading(true);

    for (let i = 0; i < files.length; i++) {
      formData.append("files", files[i]);
    }

    fileService.add(formData).then(onFilesSuccess).catch(onFilesError);
  };

  const onFilesSuccess = (res) => {
    _logger(res);
    props.handleSuccess(res);
    setIsSuccessful(true);
    setIsUploading(false);
  };

  const onFilesError = (err) => {              
    _logger(err);
    props.handleError(err);
    setIsUploading(false);
    setFailed(true);
  };

  return (
    <Fragment>
      <Grid
        container
        spacing={2}
        className="mt-1"
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Grid item xs={6} sm={3}>
          <div className="dropzone">
            <Dropzone onDrop={onDrop} multiple={props.isMultiple}>
              {({ getRootProps, getInputProps }) => (
                <div {...getRootProps()}>
                  <input {...getInputProps()} />
                  {props.isMultiple ? (
                    <div className="dz-message">
                      <div className="dx-text">
                        Drop files, or click to upload files.
                      </div>
                    </div>
                  ) : (
                    <CloudUploadIcon type="file" fontSize="large" />
                  )}
                </div>
              )}
            </Dropzone>
          </div>
        </Grid>
      </Grid>
      <Grid item xs={6} sm={3}>
        <div className="mt-3">
          {isSuccessful && (
            <MuiAlert severity="success">File Uploaded Successfully!</MuiAlert>
          )}
          {isUploading && <MuiAlert severity="info"> Uploading... </MuiAlert>}
          {failed && (
            <MuiAlert severity="warning">
              Could not upload, please try again.
            </MuiAlert>
          )}
        </div>
      </Grid>
    </Fragment>
  );
};

FileUploadDropzone.propTypes = {
  isMultiple: PropTypes.bool.isRequired,
  handleSuccess: PropTypes.func.isRequired,
  handleError: PropTypes.func.isRequired,
};

export default FileUploadDropzone;
