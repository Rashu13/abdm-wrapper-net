import React, { useState } from 'react';
import { useAbhaCreation } from '@/hooks/useAbhaCreation';
import AbhaCreationForm from '@/components/abha/AbhaCreation/AbhaCreationForm';
import OtpVerifyForm from '@/components/abha/AbhaCreation/OtpVerifyForm';
import AbhaConfirmationCard from '@/components/abha/AbhaCreation/AbhaConfirmationCard';
import AbhaIdSetupCard from '@/components/abha/AbhaCreation/AbhaIdSetupCard';
import AbhaSuccessModal from '@/components/abha/AbhaCreation/AbhaSuccessModal';
import AbhaProfileView from '@/components/abha/AbhaProfile/AbhaProfileView';
import AbhaVerificationForm from '@/components/abha/AbhaVerification/AbhaVerificationForm';

const AbhaPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'create' | 'verify'>('create');
  const creationState = useAbhaCreation();
  const {
    step,
    txnId,
    otpMessage,
    maskedMobile,
    profileData,
    isLoading,
    error,
    handleVerifyOtp,
    handleSendOtp,
    resetToAadhaarStep,
    clearError,
  } = creationState;

  const [setupStep, setSetupStep] = useState<'none' | 'confirmation' | 'id-setup'>('none');
  const [showSuccessModal, setShowSuccessModal] = useState<boolean>(false);

  const handleCustomVerifyOtp = async (otpCode: string, mobileNumber?: string) => {
    const success = await handleVerifyOtp(otpCode, mobileNumber);
    if (success) {
      setSetupStep('confirmation');
    }
    return success;
  };

  const handleAbhaIdCreated = (_newAddress: string) => {
    setSetupStep('none');
    setShowSuccessModal(true);
  };

  return (
    <main className="abha-page-main" aria-label="ABHA Create and Verify">
      {/* Top Mode Selector Navigation Bar */}
      {step === 'aadhaar-input' && setupStep === 'none' && (
        <div className="abha-top-tab-switcher">
          <button
            type="button"
            className={`abha-tab-pill ${activeTab === 'create' ? 'active' : ''}`}
            onClick={() => setActiveTab('create')}
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="16" />
              <line x1="8" y1="12" x2="16" y2="12" />
            </svg>
            <span>Create ABHA</span>
          </button>

          <button
            type="button"
            className={`abha-tab-pill ${activeTab === 'verify' ? 'active' : ''}`}
            onClick={() => setActiveTab('verify')}
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
              <polyline points="22 4 12 14.01 9 11.01" />
            </svg>
            <span>Verify ABHA</span>
          </button>
        </div>
      )}

      {/* Main Mode & Step Rendering */}
      {activeTab === 'verify' ? (
        <AbhaVerificationForm onBackToCreate={() => setActiveTab('create')} />
      ) : setupStep === 'confirmation' ? (
        <AbhaConfirmationCard
          linkedAbhaAddresses={
            profileData?.abhaAddresses && profileData.abhaAddresses.length > 0
              ? profileData.abhaAddresses
              : ['36710031510284@abdm', 'ravi.kumar.cgp@abdm']
          }
          onCreateNew={() => setSetupStep('id-setup')}
          onViewExisting={() => {
            setSetupStep('none');
            setActiveTab('verify');
          }}
        />
      ) : setupStep === 'id-setup' ? (
        <AbhaIdSetupCard onSubmit={handleAbhaIdCreated} />
      ) : step === 'otp-verify' ? (
        <OtpVerifyForm
          txnId={txnId}
          otpMessage={otpMessage}
          maskedMobile={maskedMobile}
          isLoading={isLoading}
          error={error}
          onVerifyOtp={handleCustomVerifyOtp}
          onResendOtp={handleSendOtp}
          onBack={resetToAadhaarStep}
          clearError={clearError}
        />
      ) : step === 'profile-view' ? (
        <AbhaProfileView profile={profileData} onReset={resetToAadhaarStep} />
      ) : (
        <AbhaCreationForm
          creationState={creationState}
          onToggleVerify={() => setActiveTab('verify')}
        />
      )}

      {/* ABHA ID Created Successfully Modal */}
      {showSuccessModal && (
        <AbhaSuccessModal
          onViewAbhaCard={() => {
            setShowSuccessModal(false);
            if (!profileData) {
              setActiveTab('verify');
            }
          }}
          onClose={() => setShowSuccessModal(false)}
        />
      )}
    </main>
  );
};

export default AbhaPage;
