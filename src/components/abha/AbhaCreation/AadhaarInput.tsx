import React, { useRef, useCallback, KeyboardEvent, ClipboardEvent } from 'react';
import { AadhaarInputState } from '@/types/abha.types';
import { validateAadhaar } from '@/utils/validators';
import { formatAadhaarDisplay } from '@/utils/validators';

interface AadhaarInputProps {
  value: AadhaarInputState;
  onChange: (part: 'part1' | 'part2' | 'part3', value: string) => void;
  showDigits: boolean;
  onToggleVisibility: () => void;
  disabled?: boolean;
}

const AadhaarInput: React.FC<AadhaarInputProps> = ({
  value,
  onChange,
  showDigits,
  onToggleVisibility,
  disabled = false,
}) => {
  const ref1 = useRef<HTMLInputElement>(null);
  const ref2 = useRef<HTMLInputElement>(null);
  const ref3 = useRef<HTMLInputElement>(null);

  const nextRefs: Record<string, React.RefObject<HTMLInputElement> | null> = {
    part1: ref2,
    part2: ref3,
    part3: null,
  };
  const prevRefs: Record<string, React.RefObject<HTMLInputElement> | null> = {
    part1: null,
    part2: ref1,
    part3: ref2,
  };

  const fullAadhaar = formatAadhaarDisplay(value.part1, value.part2, value.part3);
  const totalLength = fullAadhaar.length;
  const isValid = totalLength === 12 && validateAadhaar(fullAadhaar).valid;
  const hasError = totalLength === 12 && !validateAadhaar(fullAadhaar).valid;

  const handleChange = useCallback(
    (part: 'part1' | 'part2' | 'part3', inputValue: string) => {
      const digits = inputValue.replace(/\D/g, '').slice(0, 4);
      onChange(part, digits);

      if (digits.length === 4) {
        const next = nextRefs[part];
        if (next?.current) {
          next.current.focus();
        }
      }
    },
    [onChange],
  );

  const handleKeyDown = useCallback(
    (part: 'part1' | 'part2' | 'part3', e: KeyboardEvent<HTMLInputElement>) => {
      if (e.key === 'Backspace' && value[part] === '') {
        const prev = prevRefs[part];
        if (prev?.current) {
          prev.current.focus();
          const len = prev.current.value.length;
          prev.current.setSelectionRange(len, len);
        }
      }
    },
    [value],
  );

  const handlePaste = useCallback(
    (e: ClipboardEvent<HTMLInputElement>) => {
      e.preventDefault();
      const pasted = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, 12);

      if (pasted.length >= 1) {
        onChange('part1', pasted.slice(0, 4));
        onChange('part2', pasted.slice(4, 8));
        onChange('part3', pasted.slice(8, 12));

        if (pasted.length > 8) {
          ref3.current?.focus();
        } else if (pasted.length > 4) {
          ref2.current?.focus();
        }
      }
    },
    [onChange],
  );

  const getFieldClass = (_part: 'part1' | 'part2' | 'part3') => {
    const classes = ['aadhaar-box'];
    if (hasError) classes.push('error');
    if (isValid) classes.push('valid');
    return classes.join(' ');
  };

  const inputType = showDigits ? 'text' : 'password';

  return (
    <div className="aadhaar-section">
      <div className="aadhaar-boxes-container">
        {/* Box 1 */}
        <input
          ref={ref1}
          type={inputType}
          inputMode="numeric"
          pattern="\d*"
          maxLength={4}
          value={value.part1}
          onChange={(e) => handleChange('part1', e.target.value)}
          onKeyDown={(e) => handleKeyDown('part1', e)}
          onPaste={handlePaste}
          className={getFieldClass('part1')}
          placeholder="••••"
          autoComplete="off"
          disabled={disabled}
        />

        {/* Box 2 */}
        <input
          ref={ref2}
          type={inputType}
          inputMode="numeric"
          pattern="\d*"
          maxLength={4}
          value={value.part2}
          onChange={(e) => handleChange('part2', e.target.value)}
          onKeyDown={(e) => handleKeyDown('part2', e)}
          onPaste={handlePaste}
          className={getFieldClass('part2')}
          placeholder="••••"
          autoComplete="off"
          disabled={disabled}
        />

        {/* Box 3 */}
        <input
          ref={ref3}
          type={inputType}
          inputMode="numeric"
          pattern="\d*"
          maxLength={4}
          value={value.part3}
          onChange={(e) => handleChange('part3', e.target.value)}
          onKeyDown={(e) => handleKeyDown('part3', e)}
          onPaste={handlePaste}
          className={getFieldClass('part3')}
          placeholder="••••"
          autoComplete="off"
          disabled={disabled}
        />

        {/* Eye Button Box */}
        <button
          type="button"
          className="eye-box"
          onClick={onToggleVisibility}
          title={showDigits ? 'Hide digits' : 'Show digits'}
        >
          {showDigits ? (
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8">
              <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
              <line x1="1" y1="1" x2="23" y2="23" />
            </svg>
          ) : (
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8">
              <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
              <circle cx="12" cy="12" r="3" />
            </svg>
          )}
        </button>
      </div>

      {hasError && (
        <div className="aadhaar-error-msg">
          {validateAadhaar(fullAadhaar).error}
        </div>
      )}
    </div>
  );
};

export default AadhaarInput;
