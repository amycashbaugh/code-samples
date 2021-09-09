import PropTypes from "prop-types";

const wizardPropTypes = {
  userData: PropTypes.shape({
    firstName: PropTypes.string.isRequired,
    lastName: PropTypes.string.isRequired,
    password: PropTypes.string.isRequired,
    passwordConfirmation: PropTypes.string.isRequired,
    emails: PropTypes.arrayOf(PropTypes.shape({emai: PropTypes.string})),
  }),
  values: PropTypes.shape({
    firstName: PropTypes.string.isRequired,
    lastName: PropTypes.string.isRequired,
    password: PropTypes.string.isRequired,
    passwordConfirmation: PropTypes.string.isRequired,
    emails: PropTypes.arrayOf(PropTypes.shape({emai: PropTypes.string})),
  }),
  touched: PropTypes.shape({
    firstName: PropTypes.bool,
    lastName: PropTypes.bool,
    password: PropTypes.bool,
    passwordConfirmation: PropTypes.bool,
    email: PropTypes.arrayOf(PropTypes.shape({emai: PropTypes.bool})),
  }),
  errors: PropTypes.shape({
    firstName: PropTypes.string,
    lastName: PropTypes.string,
    password: PropTypes.string,
    passwordConfirmation: PropTypes.string,
    emails: PropTypes.arrayOf(PropTypes.shape({emai: PropTypes.string})),
  }),
  handleChange: PropTypes.func.isRequired,
  handleBlur: PropTypes.func.isRequired,
  handleSubmit: PropTypes.func.isRequired,
  onChange: PropTypes.func.isRequired,
  nextLabel: PropTypes.string,
  backLabel: PropTypes.string,
  onBack: PropTypes.func.isRequired,
  onNext: PropTypes.func.isRequired,
  isSubmitting: PropTypes.bool.isRequired,
  cantBack: PropTypes.bool.isRequired,
};

export { wizardPropTypes };