import React from "react";
import * as userService from "./services/userService";

class EmailConfirmation extends React.Component {
  state = {
    email: "",
    verified: "Verifying your email...",
  };

  componentDidMount() {
    let token = this.props.match.params.token;
    console.log(token);
    userService
      .emailConfirm(token)
      .then(this.onEmailVerifiedSuccess)
      .catch(this.onEmailVerifiedError);
  }

  onEmailVerifiedSuccess = (res) => {
    console.log(res);
    this.setState(() => {
      return {
        verified: "Emailed Verified Successfully!",
      };
    });
  };

  onEmailVerifiedError = (err) => {
    console.error({ error: err });
    this.setState(() => {
      return {
        verified: "Failed, please try again...",
      };
    });
  };

  render() {
    return (
      <div className="container my-5">
        <div className="row justify-content-center">
          <div className="col-6">
            <div className="card">
              <div className="card-header">
                <h1 className="text-center">{this.state.verified}</h1>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default EmailConfirmation;
