import React, { useState } from 'react';

export interface PatientConsentState {
  aadhaarSharing: boolean;
  nonAadhaarCreation: boolean;
  legacyRecordsLink: boolean;
  healthRecordsSharing: boolean;
  anonymization: boolean;
  facilityConfirmation: boolean;
  beneficiaryConsent: boolean;
  beneficiaryName: string;
}

interface ConsentSectionProps {
  consent: PatientConsentState;
  onChange: (newConsent: PatientConsentState) => void;
}

const CONSENT_ITEMS = [
  {
    key: 'aadhaarSharing' as keyof PatientConsentState,
    text: `I am voluntarily sharing my Aadhaar Number/Virtual ID issued by the Unique Identification Authority of India ("UIDAI"), and my demographic information for the purpose of creating an Ayushman Bharat Health Account number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address"). I authorize SwasthyaCare ABDM to use my Aadhaar / Virtual ID for Aadhaar based authentication with UIDAI as per Aadhaar Act, 2016.`,
  },
  {
    key: 'nonAadhaarCreation' as keyof PatientConsentState,
    text: `I intend to create an ABHA number and ABHA address using document other than Aadhaar.`,
  },
  {
    key: 'legacyRecordsLink' as keyof PatientConsentState,
    text: `I consent to usage of my ABHA address and ABHA number for linking of my legacy government health records and records generated during this encounter.`,
  },
  {
    key: 'healthRecordsSharing' as keyof PatientConsentState,
    text: `I authorize the sharing of all my health records with healthcare providers for providing healthcare services during this encounter.`,
  },
  {
    key: 'anonymization' as keyof PatientConsentState,
    text: `I consent to the anonymization and use of my health records for public health purposes.`,
  },
  {
    key: 'facilityConfirmation' as keyof PatientConsentState,
    text: `I confirm that I am visiting this facility for healthcare services and hereby authorize creation of my ABHA profile.`,
  },
];

const ConsentSection: React.FC<ConsentSectionProps> = ({ consent, onChange }) => {
  const [expandedFirstItem, setExpandedFirstItem] = useState(false);

  const handleToggleCheck = (key: keyof PatientConsentState) => {
    onChange({
      ...consent,
      [key]: !consent[key],
    });
  };

  const handleSelectAll = (checked: boolean) => {
    onChange({
      ...consent,
      aadhaarSharing: checked,
      nonAadhaarCreation: checked,
      legacyRecordsLink: checked,
      healthRecordsSharing: checked,
      anonymization: checked,
      facilityConfirmation: checked,
      beneficiaryConsent: checked,
    });
  };

  const allSelected =
    consent.aadhaarSharing &&
    consent.legacyRecordsLink &&
    consent.healthRecordsSharing &&
    consent.anonymization &&
    consent.facilityConfirmation &&
    consent.beneficiaryConsent;

  const SHORT_PREVIEW_TEXT = `I am voluntarily sharing my Aadhaar Number/Virtual ID issued by the Unique Identification Authority of India ("UIDAI"), and my demographic`;

  return (
    <div className="consent-section-wrapper">
      {/* Header with Select All */}
      <div className="consent-box-header">
        <h3 className="consent-title">Patient Consent</h3>
        <label className="select-all-label">
          <input
            type="checkbox"
            className="consent-checkbox"
            checked={allSelected}
            onChange={(e) => handleSelectAll(e.target.checked)}
          />
          <span>Select All</span>
        </label>
      </div>

      <p className="declaration-lead">I hereby declare that:</p>

      {/* Scrollable list of statements */}
      <div className="consent-scroll-area">
        {CONSENT_ITEMS.map((item, index) => {
          const isFirstItem = index === 0;
          const isChecked = !!consent[item.key];

          return (
            <div key={item.key} className={`consent-statement-item ${isChecked ? 'active' : ''}`}>
              <input
                type="checkbox"
                id={`consent-${item.key}`}
                className="consent-checkbox"
                checked={isChecked}
                onChange={() => handleToggleCheck(item.key)}
              />
              <label htmlFor={`consent-${item.key}`} className="statement-text">
                {isFirstItem ? (
                  expandedFirstItem ? (
                    <>
                      {item.text}{' '}
                      <button
                        type="button"
                        className="consent-toggle-btn"
                        onClick={(e) => {
                          e.stopPropagation();
                          setExpandedFirstItem(false);
                        }}
                      >
                        Read Less
                      </button>
                    </>
                  ) : (
                    <>
                      {SHORT_PREVIEW_TEXT}{' '}
                      <button
                        type="button"
                        className="consent-toggle-btn"
                        onClick={(e) => {
                          e.stopPropagation();
                          setExpandedFirstItem(true);
                        }}
                      >
                        ...Read More
                      </button>
                    </>
                  )
                ) : (
                  item.text
                )}
              </label>
            </div>
          );
        })}
      </div>

      {/* Beneficiary Name Input Field */}
      <div className="beneficiary-input-block">
        <p className="beneficiary-instruction">Fill Beneficiary Details (as on Aadhaar):</p>
        <div className="beneficiary-declaration-row">
          <input
            type="checkbox"
            id="beneficiary-explained-cb"
            className="consent-checkbox"
            checked={consent.beneficiaryConsent}
            onChange={() => handleToggleCheck('beneficiaryConsent')}
          />
          <label htmlFor="beneficiary-explained-cb" className="declaration-inputs">
            <span>I,</span>
            <input
              type="text"
              id="beneficiary-name-input"
              className="beneficiary-name-input"
              placeholder="Enter Beneficiary Name"
              value={consent.beneficiaryName || ''}
              onChange={(e) =>
                onChange({
                  ...consent,
                  beneficiaryName: e.target.value,
                })
              }
            />
            <span>have been explained about the consent and hereby provide my consent.</span>
          </label>
        </div>
      </div>
    </div>
  );
};

export default ConsentSection;
