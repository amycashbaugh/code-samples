import React, { useEffect } from "react";
import { withFormik, FieldArray, ErrorMessage } from "formik";
import "./Wizard.css";
import wizardSchema from "../../schemas/wizardSchema";
import * as wizardProps from "./wizardPropTypes";
import { Grid, Card, Divider, Button, TextField, Fab } from "@material-ui/core";
import DeleteIcon from "@material-ui/icons/Delete";

const WizardStep3 = (props) => {
  const {
    values,
    touched,
    errors,
    handleChange,
    handleBlur,
    handleSubmit,
    onBack,
    isSubmitting,
    cantBack,
    backLabel,
    nextLabel,
    onFinish,
  } = props;

  useEffect(() => {
    onChange();
  }, [values]);

  const onChange = () => {
    props.onChange(values);
  };

  const onBackClicked = () => {
    onBack(values);
  };

  return (
    <form onSubmit={handleSubmit} className="p-1">
      <Card className="p-4 mb-4">
        <div className="font-size-lg font-weight-bold">Email</div>
        <Divider className="my-4" />
        <Grid container spacing={4}>
          <Grid item xs={12} lg={6}>
            <FieldArray name="emails">
              {({ push, remove }) => (
                <div className="mb-5">
                  {" "}
                  <Button
                    className="btn btn-success mx-2"
                    onClick={() => push({ email: "" })}
                  >
                    +
                  </Button>
                  {values.emails.length > 0 &&
                    values.emails.map((email, index) => (
                      <div className="row" key={index}>
                        <TextField
                          fullWidth
                          className="m-2"
                          type="text"
                          id="email"
                          name={`emails.${index}.email`}
                          placeholder="Add an Email"
                          value={values.emails[index].email}
                          onChange={handleChange}
                          onBlur={handleBlur}
                          variant="outlined"
                          error={touched.emails && Boolean(errors.emails)}
                          InputProps={{
                            endAdornment: (
                              <Fab
                                className="btn btn-danger mx-2"
                                size="small"
                                style={{ float: "right" }}
                                onClick={() => remove(index)}
                              >
                                <DeleteIcon />
                              </Fab>
                            ),
                          }}
                        />

                        <ErrorMessage name={`emails.${index}.email`} />
                      </div>
                    ))}
                </div>
              )}
            </FieldArray>
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
                disabled={!values.email && Boolean(errors.email)}
                onClick={onFinish}
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

WizardStep3.propTypes = wizardProps.wizardPropTypes;

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
})(WizardStep3);
