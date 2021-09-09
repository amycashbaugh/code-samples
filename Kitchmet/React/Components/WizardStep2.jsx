import React, { useEffect } from "react";
import { withFormik } from "formik";
import "./Wizard.css";
import wizardSchema from "../../schemas/wizardSchema";
import * as wizardProps from "./wizardPropTypes";
import { Grid, Card, TextField, Divider, Button } from "@material-ui/core";
import logger from "sabio-debug";

const _logger = logger.extend("Wizard2");

const WizardStep2 = (props) => {
  const {
    values,
    touched,
    errors,
    handleChange,
    handleBlur,
    handleSubmit,
    onBack,
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

  const onBackClicked = () => {
    onBack(values);
    _logger(values);
  };

  return (
    <form onSubmit={handleSubmit} className="p-1">
      <Card className="p-4 mb-4">
        <div className="font-size-lg font-weight-bold">
          Password Confirmation
        </div>
        <Divider className="my-4" />
        <Grid container spacing={4}>
          <Grid item xs={12} lg={6}>
            <div className="form-group">
              <label htmlFor="password">Password </label>
              <TextField
                fullWidth
                id="password"
                type="password"
                name="password"
                value={values.password}
                onChange={handleChange}
                onBlur={handleBlur}
                variant="outlined"
                error={touched.password && Boolean(errors.password)}
                helperText={touched.password && errors.password}
              />
              <div className="form-group pt-4">
                <label htmlFor="passwordConfirmation">Confirm Password </label>
                <TextField
                  fullWidth
                  name="passwordConfirmation"
                  id="passwordConfirmation"
                  type="password"
                  value={values.passwordConfirmation}
                  onChange={handleChange}
                  onBlur={handleBlur}
                  variant="outlined"
                  error={
                    touched.passwordConfirmation &&
                    Boolean(errors.passwordConfirmation)
                  }
                  helperText={
                    touched.passwordConfirmation && errors.passwordConfirmation
                  }
                />
              </div>
            </div>
            <div className="button-group pt-3">
              <Button
                type="button"
                className="btn btn-secondary"
                onClick={onBackClicked}
                disabled={isSubmitting || cantBack}
              >
                {backLabel}
              </Button>
              <Button
                type="submit"
                className="btn btn-primary ml-1"
                disabled={
                  !values.password ||
                  Boolean(errors.password) ||
                  !values.passwordConfirmation ||
                  Boolean(errors.passwordConfirmation)
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

WizardStep2.propTypes = wizardProps.wizardPropTypes;

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
})(WizardStep2);
