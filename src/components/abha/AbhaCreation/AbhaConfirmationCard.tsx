import React from 'react';

interface AbhaConfirmationCardProps {
  linkedAbhaAddresses?: string[];
  onCreateNew: () => void;
  onViewExisting: () => void;
}

const AbhaConfirmationCard: React.FC<AbhaConfirmationCardProps> = ({
  linkedAbhaAddresses = ['36710031510284@abdm', 'ravi.kumar.cgp@abdm'],
  onCreateNew,
  onViewExisting,
}) => {
  return (
    <div className="w-full max-w-xl mx-auto my-6">
      <div className="bg-white border-1.5 border-slate-200/90 rounded-2xl shadow-sm overflow-hidden p-6 md:p-8 flex flex-col gap-6">
        {/* Top Header Banner Box */}
        <div className="bg-blue-50/80 border border-blue-100/80 rounded-2xl p-5 flex items-center justify-center gap-3.5">
          <div className="w-10 h-10 rounded-full bg-[#0f2942] text-white flex items-center justify-center font-black text-xl shrink-0">
            ?
          </div>
          <h2 className="text-xl font-bold text-slate-800 tracking-tight m-0">
            Confirmation Required
          </h2>
        </div>

        {/* Info Body Text Box */}
        <div className="bg-slate-50/90 border border-slate-100 rounded-xl p-5 text-sm font-semibold text-slate-700 leading-relaxed flex flex-col gap-2">
          <p className="m-0">
            ABHA addresses already exist for this Aadhaar. The following ABHA addresses are linked:
          </p>
          <p className="m-0 font-bold text-slate-900 break-all">
            {linkedAbhaAddresses.join(', ')}
          </p>
          <p className="m-0 pt-1">
            Do you want to create new ABHA Address?
          </p>
        </div>

        {/* Bottom Two Action Buttons */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-2">
          <button
            type="button"
            className="w-full bg-white border-1.5 border-slate-300 hover:bg-slate-50 text-slate-800 font-bold py-3.5 px-6 rounded-xl text-base transition-all duration-200 cursor-pointer text-center"
            onClick={onCreateNew}
          >
            Yes, Create New
          </button>

          <button
            type="button"
            className="w-full bg-brand hover:bg-brand-dark text-white font-bold py-3.5 px-6 rounded-xl text-base shadow-md transition-all duration-200 cursor-pointer text-center"
            onClick={onViewExisting}
          >
            No, View Existing
          </button>
        </div>
      </div>
    </div>
  );
};

export default AbhaConfirmationCard;
