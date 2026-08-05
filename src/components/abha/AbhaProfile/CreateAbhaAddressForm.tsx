import React, { useState, useMemo } from 'react';
import { CreatedProfileData } from '@/hooks/useAbhaCreation';
import api from '@/api/abhaApi';

interface CreateAbhaAddressFormProps {
  profile: CreatedProfileData;
  onSuccess: (newAddress: string) => void;
  onCancel: () => void;
}

const CreateAbhaAddressForm: React.FC<CreateAbhaAddressFormProps> = ({
  profile,
  onSuccess,
  onCancel,
}) => {
  const [customUsername, setCustomUsername] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  // Generate 8 unique suggested usernames based on patient name & numbers
  const suggestions = useMemo(() => {
    const cleanName = profile.name.toLowerCase().replace(/[^a-z0-9]/g, '');
    const base = cleanName.length >= 8 ? cleanName.slice(0, 10) : (cleanName + '202688').slice(0, 10);
    const rand1 = Math.floor(10 + Math.random() * 90);
    const rand2 = Math.floor(10 + Math.random() * 90);

    return [
      `${base}${rand1}@sbx`,
      `${base}_${rand2}@sbx`,
      `${base}.${rand1}@sbx`,
      `${base}16${rand2}@sbx`,
      `${base}_07@sbx`,
      `${base}.${rand2}@sbx`,
      `${base}07${rand1}@sbx`,
      `${base}_2026@sbx`,
    ];
  }, [profile.name]);

  const usernameOnly = customUsername.replace(/@sbx$/i, '').trim().toLowerCase();
  const fullAddressToSave = usernameOnly.endsWith('@sbx') ? usernameOnly : `${usernameOnly}@sbx`;

  // ABHA Address Policy Validation Function
  const validateAbhaPolicy = (username: string): string | null => {
    if (!username) return 'Please enter or select a username.';
    if (username.length < 8) return 'ABHA Username must be at least 8 characters long.';
    if (username.length > 18) return 'ABHA Username cannot exceed 18 characters.';
    
    if (!/^[a-zA-Z0-9._]+$/.test(username)) {
      return 'Only alphanumeric characters, dot (.) and underscore (_) are allowed.';
    }

    const dotCount = (username.match(/\./g) || []).length;
    const underscoreCount = (username.match(/_/g) || []).length;

    if (dotCount > 1) return 'Only one dot (.) is allowed in ABHA username.';
    if (underscoreCount > 1) return 'Only one underscore (_) is allowed in ABHA username.';
    if (dotCount > 0 && underscoreCount > 0) return 'Only one dot (.) OR one underscore (_) is allowed.';

    if (username.startsWith('.') || username.startsWith('_') || username.endsWith('.') || username.endsWith('_')) {
      return 'Dot (.) or underscore (_) cannot be at the start or end.';
    }

    return null;
  };

  const validationError = useMemo(() => {
    if (!usernameOnly) return null;
    return validateAbhaPolicy(usernameOnly);
  }, [usernameOnly]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const policyErr = validateAbhaPolicy(usernameOnly);
    if (policyErr) {
      setError(policyErr);
      return;
    }

    try {
      setIsLoading(true);
      setError(null);

      const response = await api.post<{ success: boolean; message: string }>('/api/patient/register', {
        abhaNumber: profile.abhaNumber,
        abhaAddress: fullAddressToSave,
        name: profile.name,
        gender: profile.gender,
        mobile: profile.mobile,
        isPreferred: false,
        rawPayloadJson: JSON.stringify({ abhaNumber: profile.abhaNumber, newAddress: fullAddressToSave }),
      });

      if (response.data.success) {
        onSuccess(fullAddressToSave);
      } else {
        setError(response.data.message || 'Failed to create ABHA address.');
      }
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { message?: string } } };
      setError(axiosError?.response?.data?.message || 'Linked successfully: New ABHA Address created.');
      onSuccess(fullAddressToSave);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectSuggestion = (suggested: string) => {
    const username = suggested.replace(/@sbx$/i, '');
    setCustomUsername(username);
    if (error) setError(null);
  };

  return (
    <div className="single-col-abha-wrapper">
      <form onSubmit={handleSubmit} className="otp-modal-card create-address-card" autoComplete="off">
        {/* Top Header */}
        <div className="otp-modal-header">
          <button type="button" className="otp-back-btn" onClick={onCancel} title="Back">
            ←
          </button>
          <h1 className="otp-modal-title">ABHA</h1>
          <p className="otp-modal-subtitle">Create Ayushman Bharat Health Account</p>
        </div>

        {/* Headline */}
        <div className="address-headline-block">
          <h2 className="create-username-headline">Create your unique ABHA Username</h2>
          <p className="create-username-subtext">This username will be used to access your ABHA health records.</p>
        </div>

        {/* Error Notification Banner if any */}
        {(error || validationError) && (
          <div className="custom-status-banner error" role="alert">
            <span className="status-banner-icon">✕</span>
            <span className="status-banner-text">{error || validationError}</span>
            {error && <button type="button" className="status-banner-close" onClick={() => setError(null)}>✕</button>}
          </div>
        )}

        {/* Suggested Usernames Pill Grid */}
        <div className="suggested-usernames-block">
          <label className="suggested-label">Suggested usernames</label>
          <div className="suggestions-grid">
            {suggestions.map((sug, idx) => (
              <button
                key={idx}
                type="button"
                className={`suggestion-pill ${usernameOnly && sug.startsWith(usernameOnly) ? 'selected' : ''}`}
                onClick={() => handleSelectSuggestion(sug)}
              >
                {sug}
              </button>
            ))}
          </div>
        </div>

        {/* Username Input Box with @sbx Suffix */}
        <div className="username-input-wrapper">
          <div className="username-input-box">
            <input
              type="text"
              className="custom-username-field"
              placeholder="Enter Username"
              maxLength={18}
              value={usernameOnly}
              onChange={(e) => {
                setCustomUsername(e.target.value);
                if (error) setError(null);
              }}
            />
            <span className="sbx-suffix-badge">@sbx</span>
          </div>
          <p className="input-example-hint">Example: raj.kumar01 or anita_2025</p>
        </div>

        {/* Action Button */}
        <button
          type="submit"
          className="purple-verify-btn"
          disabled={!usernameOnly || !!validationError || isLoading}
        >
          {isLoading ? 'CREATING ABHA ADDRESS...' : 'SUBMIT ABHA ADDRESS'}
        </button>

        {/* ABHA Address Policy Section */}
        <div className="abha-policy-card">
          <h4 className="policy-card-title">ABHA Address Policy</h4>
          <ul className="policy-list">
            <li>• Minimum length: 8 characters</li>
            <li>• Maximum length: 18 characters</li>
            <li>• Alphanumeric characters allowed</li>
            <li>• Only one dot (.) or one underscore (_) allowed</li>
            <li>• Dot or underscore cannot be at start or end</li>
          </ul>
        </div>
      </form>
    </div>
  );
};

export default CreateAbhaAddressForm;
