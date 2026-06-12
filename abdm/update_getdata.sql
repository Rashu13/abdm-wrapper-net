
ALTER PROCEDURE [dbo].[GetDataOnUHIDForAll]
@OP int,
@RegNo int,
@SessionId int
AS
BEGIN
    if @OP=1
    Begin
        if Exists (Select RegNo,SessionId from tblOPD Where RegNo=@RegNo and SessionId=@SessionId)
            Select R.Name,Isnull(R.DOB,GetDate()) DOB,R.Age,R.[Age In],R.Sex,R.FName,R.ContactNo,R.Address,R.Weight,R.[Weight In],R.Temp,D.Name DName,A.OPDType,B.Name RefDoc,R.PatientType,max(A.ValidUpto) ValidUpto,R.Type,T.Name TPA,D.DID,A.Reason,R.City,R.State,R.AadharNo,A.PType,A.LMP,A.EDD,R.RDID,R.MemberType 'Whatsapp', R.ABHANumber, R.ABHAAddress 
            from tblRegistration R,tblOPD A,tblDoc D,tblRefDoc B,tblTPA T 
            Where R.RegNo=A.RegNo and R.SessionId=A.SessionId and A.CDID=D.DID and R.RDId=B.RefId and A.TPAID=T.TID and R.RegNo=@RegNo and A.SrNo in (Select Max(SrNo) from tblOPD Where RegNo=@RegNo and SessionId=@SessionId) and R.SessionId=@SessionId 
            Group By R.Name,R.DOB,R.Age,R.[Age In],R.Sex,R.FName,R.MName,R.ContactNo,R.Address,R.Weight,R.[Weight In],R.Temp,D.Name,A.OPDType,B.Name,R.PatientType,R.Type,T.Name,D.DID,A.Reason,R.City,R.State,R.AadharNo,A.PType,A.LMP,A.EDD,R.RDID,R.MemberType, R.ABHANumber, R.ABHAAddress
        else
            Select R.Name,getdate() DOB,R.Age,R.[Age In],R.Sex,R.FName,R.ContactNo,R.Address,R.Weight,R.[Weight In],R.Temp,D.Name DName,A.OPDType,B.Name RefDoc,R.PatientType,max(A.ValidUpto) ValidUpto,R.Type,T.Name TPA,D.DID,A.Reason,R.City,R.State,R.AadharNo,A.PType,A.LMP,A.EDD,R.RDID,R.MemberType 'Whatsapp', R.ABHANumber, R.ABHAAddress 
            from tblRegistration R,tblOPD A,tblDoc D,tblRefDoc B,tblTPA T 
            Where R.RegNo=A.RegNo and R.SessionId=A.SessionId and A.CDID=D.DID and R.RDId=B.RefId and A.TPAID=T.TID and R.RegNo=@RegNo 
            Group By R.Name,R.DOB,R.Age,R.[Age In],R.Sex,R.FName,R.MName,R.ContactNo,R.Address,R.Weight,R.[Weight In],R.Temp,D.Name,A.OPDType,B.Name,R.PatientType,R.Type,T.Name,D.DID,A.Reason,R.City,R.State,R.AadharNo,A.PType,A.LMP,A.EDD,R.RDID,R.MemberType, R.ABHANumber, R.ABHAAddress
    End
END
