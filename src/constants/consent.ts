// ─── Consent Text — Single Source of Truth (NHA/ABDM Standard) ────────────────

export interface ConsentBlock {
  id: string;
  shortText: string;
  fullText: string;
  required: boolean;
}

export const CONSENT_BLOCKS: ConsentBlock[] = [
  {
    id: 'aadhaar-consent',
    shortText:
      'I am voluntarily sharing my Aadhaar Number / Virtual ID issued by UIDAI, and my demographic information for the purpose of creating an ABHA number and ABHA Address. I authorize to use my Aadhaar number / Virtual ID for performing Aadhaar based authentication with UIDAI...',
    fullText:
      'I am voluntarily sharing my Aadhaar Number / Virtual ID issued by the Unique Identification Authority of India ("UIDAI"), and my demographic information for the purpose of creating an Ayushman Bharat Health Account number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address"). I authorize to use my Aadhaar number / Virtual ID for performing Aadhaar based authentication with UIDAI as per the provisions of the Aadhaar (Targeted Delivery of Financial and other Subsidies, Benefits and Services) Act, 2016 for the aforesaid purpose. I understand that UIDAI will share my e-KYC details, or response of "Yes" with upon successful authentication.',
    required: true,
  },
  {
    id: 'other-document-consent',
    shortText:
      'I intend to create Ayushman Bharat Health Account Number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address") using document other than Aadhaar. (Click here to proceed further)',
    fullText:
      'I intend to create Ayushman Bharat Health Account Number ("ABHA number") and Ayushman Bharat Health Account address ("ABHA Address") using document other than Aadhaar. (Click here to proceed further)',
    required: false,
  },
  {
    id: 'legacy-health-records-consent',
    shortText:
      'I consent to usage of my ABHA address and ABHA number for linking of my legacy (past) health records and those which will be generated during this encounter.',
    fullText:
      'I consent to usage of my ABHA address and ABHA number for linking of my legacy (past) health records and those which will be generated during this encounter.',
    required: true,
  },
  {
    id: 'health-record-share-consent',
    shortText:
      'I authorize the sharing of all my health records with healthcare provider(s) for the purpose of providing healthcare services to me during this encounter.',
    fullText:
      'I authorize the sharing of all my health records with healthcare provider(s) for the purpose of providing healthcare services to me during this encounter.',
    required: true,
  },
  {
    id: 'anonymization-consent',
    shortText:
      'I consent to the anonymization and subsequent use of my health records for public health purposes.',
    fullText:
      'I consent to the anonymization and subsequent use of my health records for public health purposes.',
    required: true,
  },
];

export const BENEFICIARY_CONSENT_TEXT = {
  informed:
    'I, confirm that I have duly informed and explained the beneficiary of the contents of consent for aforementioned purposes.',
  explained:
    'I have been explained about the consent as stated above and hereby provide my consent for the aforementioned purposes.',
};

export const CONSENT_HEADING = 'Terms and Conditions';
