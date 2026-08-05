import React from 'react';

interface AbhaSuccessModalProps {
  onViewAbhaCard: () => void;
  onClose?: () => void;
}

const AbhaSuccessModal: React.FC<AbhaSuccessModalProps> = ({
  onViewAbhaCard,
  onClose,
}) => {
  return (
    <div className="fixed inset-0 z-50 bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4">
      <div className="bg-white rounded-3xl p-8 md:p-10 max-w-md w-full shadow-2xl relative text-center flex flex-col items-center gap-6 animate-in fade-in zoom-in-95 duration-200">
        {/* Close Icon Button */}
        {onClose && (
          <button
            type="button"
            className="absolute top-5 right-6 text-slate-400 hover:text-slate-600 text-xl font-bold transition-colors cursor-pointer"
            onClick={onClose}
          >
            ✕
          </button>
        )}

        {/* Large Circular Checkmark Icon */}
        <div className="w-20 h-20 rounded-full bg-brand text-white flex items-center justify-center shadow-lg my-2">
          <svg className="w-10 h-10" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
          </svg>
        </div>

        {/* Title & Description */}
        <div className="flex flex-col gap-2">
          <h2 className="text-2xl font-black text-slate-800 tracking-tight m-0">
            ABHA ID Created Successfully
          </h2>
          <p className="text-sm font-semibold text-slate-500 leading-relaxed m-0 pt-1">
            Your ABHA (Ayushman Bharat Health Account) has been created successfully.
            <br />
            You can now download your ABHA card.
          </p>
        </div>

        {/* Action Button: View ABHA Card */}
        <button
          type="button"
          className="w-full bg-brand hover:bg-brand-dark text-white font-bold py-3.5 px-8 rounded-xl text-base shadow-md transition-all duration-200 cursor-pointer mt-2"
          onClick={onViewAbhaCard}
        >
          View ABHA Card
        </button>
      </div>
    </div>
  );
};

export default AbhaSuccessModal;
