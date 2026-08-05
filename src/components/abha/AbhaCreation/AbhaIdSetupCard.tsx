import React, { useState } from 'react';

interface AbhaIdSetupCardProps {
  suggestions?: string[];
  onSubmit?: (newAbhaAddress: string) => void;
  isLoading?: boolean;
}

const DEFAULT_SUGGESTIONS = [
  'ravikumar27031994',
  'ravikumar03199427',
  'ravi_ravi270394',
  'ravi_ravi270319',
  'ravi_ravi19942703',
  'ravi_ravi1994.2727',
  'ravi_ravi1994.2703',
];

const AbhaIdSetupCard: React.FC<AbhaIdSetupCardProps> = ({
  suggestions = DEFAULT_SUGGESTIONS,
  onSubmit,
  isLoading = false,
}) => {
  const [abhaAddress, setAbhaAddress] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (abhaAddress.trim().length >= 8 && !isLoading && onSubmit) {
      onSubmit(abhaAddress.trim());
    }
  };

  return (
    <div className="w-full max-w-xl mx-auto my-4">
      <form
        onSubmit={handleSubmit}
        className="bg-white border-1.5 border-slate-200/90 rounded-2xl shadow-sm overflow-hidden"
        autoComplete="off"
      >
        {/* Header Bar */}
        <div className="bg-slate-50/80 border-b border-slate-100 px-7 py-4.5">
          <h2 className="text-xl font-bold text-slate-800 tracking-tight m-0">
            ABHA ID Setup
          </h2>
        </div>

        <div className="p-6 md:p-7 flex flex-col gap-5">
          <p className="text-sm font-semibold text-slate-500 m-0">
            Choose a username to set up your ABHA ID
          </p>

          {/* Fieldset Outlined Floating-Legend Input Box */}
          <fieldset className="border-1.5 border-slate-300 rounded-xl px-4 pb-2.5 pt-0 flex items-center justify-between focus-within:border-brand focus-within:ring-1 focus-within:ring-brand transition-all relative">
            <legend className="px-2 text-xs font-semibold text-slate-400 ml-1">
              Your ABHA Address
            </legend>
            <input
              type="text"
              className="w-full bg-transparent border-none outline-none text-slate-800 placeholder-slate-400 font-semibold text-base py-1"
              placeholder="Enter ABHA Address"
              value={abhaAddress}
              onChange={(e) => setAbhaAddress(e.target.value)}
              disabled={isLoading}
              autoFocus
            />
          </fieldset>

          {/* Validation Rule Info Box */}
          <div className="bg-slate-50/90 border border-slate-100/90 rounded-xl p-4.5 flex flex-col gap-2.5">
            <p className="text-xs font-bold text-slate-700 m-0">
              ABHA Address Validation rule:
            </p>
            <ul className="text-xs font-semibold text-rose-500 flex flex-col gap-1.5 pl-4 m-0 list-disc">
              <li>Minimum length 8, Maximum length 18 characters</li>
              <li>Allowed special characters: 1 dot (.) and/or 1 underscore (_)</li>
              <li>Special characters cannot be at the beginning or end</li>
              <li>Only alphanumeric characters allowed (with . and _)</li>
            </ul>
          </div>

          {/* Suggestions List */}
          <div className="flex flex-col gap-2.5">
            <p className="text-xs font-bold text-slate-500 m-0">Suggestions:</p>
            <div className="flex flex-wrap gap-2 max-h-40 overflow-y-auto pr-1">
              {suggestions.map((sug) => (
                <button
                  key={sug}
                  type="button"
                  className={`px-4 py-2 rounded-full text-xs font-bold border transition-all duration-150 cursor-pointer ${
                    abhaAddress === sug
                      ? 'bg-brand text-white border-brand shadow-sm'
                      : 'bg-white border-slate-300 text-slate-700 hover:bg-slate-50 hover:border-slate-400'
                  }`}
                  onClick={() => setAbhaAddress(sug)}
                >
                  {sug}
                </button>
              ))}
            </div>
          </div>

          {/* Action Button: Create ABHA ID */}
          <div className="pt-2">
            <button
              type="submit"
              className="w-full bg-brand hover:bg-brand-dark disabled:bg-slate-300 text-white font-bold py-3.5 px-6 rounded-xl text-base shadow-md transition-all duration-200 flex items-center justify-center gap-2 cursor-pointer disabled:cursor-not-allowed"
              disabled={abhaAddress.trim().length < 8 || isLoading}
            >
              {isLoading ? (
                <>
                  <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  <span>Creating ABHA ID...</span>
                </>
              ) : (
                'Create ABHA ID'
              )}
            </button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default AbhaIdSetupCard;
