//THIS IS A REUSABLE WIZARD STEP FORM

import "../forms/Wizard.css";
import React, { useState } from "react";
import Loki from "react-loki";
import WizardStep1 from "./WizardStep1";
import WizardStep2 from "./WizardStep2";
import WizardStep3 from "./WizardStep3";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faLock, faUser, faEnvelope } from "@fortawesome/free-solid-svg-icons";

const Wizard = () => {
  const [userData, setUserData] = useState({
    firstName: "",
    lastName: "",
    password: "",
    passwordConfirmation: "",
    emails: [{ email: "" }],
  });

  const onChange = (values) => {
    setUserData(values);
  };

  const onFinish = (values) => {
    setUserData(values);
    //ajax call here
  };

  const wizardSteps = [
    {
      label: "Step 1",
      icon: <FontAwesomeIcon icon={faUser} className="mt-3" />,
      component: <WizardStep1 userData={userData} onChange={onChange} />,
    },
    {
      label: "Step 2",
      icon: <FontAwesomeIcon icon={faLock} className="mt-3" />,
      component: <WizardStep2 userData={userData} onChange={onChange} />,
    },
    {
      label: "Step 3",
      icon: <FontAwesomeIcon icon={faEnvelope} className="mt-3" />,
      component: <WizardStep3 userData={userData} onChange={onChange} />,
    },
  ];

  return (
    <div className="wizard">
      <Loki
        steps={wizardSteps}
        onNext={onChange}
        onBack={onChange}
        onFinish={onFinish}
        nextLabel="Next"
        backLabel="Back"
        noActions
      />
    </div>
  );
};

export default Wizard;
