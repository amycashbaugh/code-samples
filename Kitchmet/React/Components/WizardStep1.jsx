import React, { useEffect } from "react";
import { withFormik } from "formik";
import wizardSchema from "../../schemas/wizardSchema";
import * as wizardProps from "./wizardPropTypes";
import { Grid, Card, TextField, Divider, Button } from "@material-ui/core";
import logger from "sabio-debug";

const _logger = logger.extend("Wizard1");

const WizardStep1 = (props) => {
  const {
    values,
    touched,
    errors,
    handleChange,
    handleBlur,
    handleSubmit,
    onNext,
    isSubmitting,
    cantBack,
    backLabel,
    nextLabel,
  } = props;

  useEffect(() => {
    onChange();
  }, [values]);

  const onChange = () => {
    props.onChange(values);
    _logger(values);
  };

  const onNextClicked = () => {
    onNext(values);
    _logger(values);
  };

  return (
    <form onSubmit={handleSubmit} className="p-1">
      <Card className="p-4 mb-4">
        <div className="font-size-lg font-weight-bold">User Information</div>
        <Divider className="my-4" />
        <Grid container spacing={4}>
          <Grid item xs={12} lg={6}>
            <div className="form-group">
              <label htmlFor="firstName">First Name </label>
              <TextField
                fullWidth
                id="firstName"
                name="firstName"
                value={values.firstName}
                onChange={handleChange}
                onBlur={handleBlur}
                variant="outlined"
                error={touched.firstName && Boolean(errors.firstName)}
                helperText={touched.firstName && errors.firstName}
              />
              <div className="form-group pt-4">
                <label htmlFor="lastName">Last Name </label>
                <TextField
                  fullWidth
                  name="lastName"
                  id="lastName"
                  value={values.lastName}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  variant="outlined"
                  error={touched.lastName && Boolean(errors.lastName)}
                  helperText={touched.lastName && errors.lastName}
                />
              </div>
            </div>
            <div className="button-group pt-3">
              <Button
                type="button"
                className="btn btn-secondary"
                disabled={isSubmitting || cantBack}
              >
                {backLabel}
              </Button>
              <Button
                type="submit"
                className="btn btn-primary ml-1"
                disabled={
                  !values.firstName ||
                  Boolean(errors.firstName) ||
                  !values.lastName ||
                  Boolean(errors.lastName)
                }
                onClick={onNextClicked}
              >
                {nextLabel}
              </Button>
            </div>
          </Grid>
        </Grid>
      </Card>
    </form>
  );
};

WizardStep1.propTypes = wizardProps.wizardPropTypes;

export default withFormik({
  mapPropsToValues: (props) => ({
    firstName: props.userData.firstName,
    lastName: props.userData.lastName,
    password: props.userData.password,
    passwordConfirmation: props.userData.passwordConfirmation,
    emails: props.userData.emails,
  }),
  validationSchema: wizardSchema,
  handleSubmit: (values, { props }) => {
    props.onNext(values);
  },
})(WizardStep1);
