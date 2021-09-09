import React, { Fragment } from "react";
//import { PageTitle } from "../../layout-components";
import { WrapperSimple } from "../../layout-components";
import FileUploadDropzone from "./FileUploadDropzone";

export default function FileUpload() {
  //place your own logic in the handlers if you need any information return to you from the file uploader.
  const handleSuccess = () => {};
  const handleError = () => {};

  return (
    <Fragment>
      {/* <PageTitle
        titleHeading="Upload"
        titleDescription="Handle your file uploads on the server easily and with style."
      /> */}

      <WrapperSimple sectionHeading="File Upload">
        <FileUploadDropzone
          isMultiple={true} //  'false' for a single file uploader or 'true' for multiple files
          handleSuccess={handleSuccess}
          handleError={handleError}
        />
      </WrapperSimple>
    </Fragment>
  );
}
