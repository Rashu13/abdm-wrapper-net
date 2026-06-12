CREATE PROCEDURE [dbo].[IUDOPD]
                                                                                                                                                                                                                              
@OP int,
                                                                                                                                                                                                                                                     
@SrNo int,
                                                                                                                                                                                                                                                   
@RegNo int,
                                                                                                                                                                                                                                                  
@OPDNo int,
                                                                                                                                                                                                                                                  
@OPDDate Datetime,
                                                                                                                                                                                                                                           
@Name Nvarchar(50),
                                                                                                                                                                                                                                          
@DOB nvarchar(50),
                                                                                                                                                                                                                                           
@Age Nvarchar(50),
                                                                                                                                                                                                                                           
@AgeIn Nvarchar(50),
                                                                                                                                                                                                                                         
@Sex Nvarchar(50),
                                                                                                                                                                                                                                           
@Type Nvarchar(50),
                                                                                                                                                                                                                                          
@FName Nvarchar(50),
                                                                                                                                                                                                                                         
@MName Nvarchar(50),
                                                                                                                                                                                                                                         
@COntactNo nvarchar(50),
                                                                                                                                                                                                                                     
@Address nvarchar(max),
                                                                                                                                                                                                                                      
@Weight nvarchar(50),
                                                                                                                                                                                                                                        
@WeightIn Nvarchar(50),
                                                                                                                                                                                                                                      
@Temp Nvarchar(50),
                                                                                                                                                                                                                                          
@CDID int,
                                                                                                                                                                                                                                                   
@RDID int,
                                                                                                                                                                                                                                                   
@OPDCharges Decimal(18,0),
                                                                                                                                                                                                                                   
@Problem Nvarchar(50),
                                                                                                                                                                                                                                       
@Status bit,
                                                                                                                                                                                                                                                 
@Date nvarchar(50),
                                                                                                                                                                                                                                          
@OPDType nvarchar(50),
                                                                                                                                                                                                                                       
@Complaints nvarchar(max),
                                                                                                                                                                                                                                   
@FollowUp nvarchar(max),
                                                                                                                                                                                                                                     
@UserId int,
                                                                                                                                                                                                                                                 
@ValidUpto Datetime,
                                                                                                                                                                                                                                         
@Note nvarchar(max),
                                                                                                                                                                                                                                         
@IsMember bit,
                                                                                                                                                                                                                                               
@MemberShipNo int,
                                                                                                                                                                                                                                           
@MemberType nvarchar(50),
                                                                                                                                                                                                                                    
@SessionId int,
                                                                                                                                                                                                                                              
@Refund decimal(18,2),
                                                                                                                                                                                                                                       
@Reason nvarchar(max),
                                                                                                                                                                                                                                       
@PatientType nvarchar(50),
                                                                                                                                                                                                                                   
@FileCharges Decimal(18,2),
                                                                                                                                                                                                                                  
@FeeType	nvarchar(50),
                                                                                                                                                                                                                                       
@BP NVARCHAR(50),
                                                                                                                                                                                                                                            
@PLUSE NVARCHAR (50),
                                                                                                                                                                                                                                        
@PaymentMode nvarchar(50),
                                                                                                                                                                                                                                   
@Balance float,
                                                                                                                                                                                                                                              
@OPDFee float,
                                                                                                                                                                                                                                               
@MText nvarchar(50),
                                                                                                                                                                                                                                         
@Online float,
                                                                                                                                                                                                                                               
@TPAID int,
                                                                                                                                                                                                                                                  
@State nvarchar(25),
                                                                                                                                                                                                                                         
@City nvarchar(25),
                                                                                                                                                                                                                                          
@BGS bit,
                                                                                                                                                                                                                                                    
@AadharNo nvarchar(20),
                                                                                                                                                                                                                                      
@PType nvarchar(50),
                                                                                                                                                                                                                                         
@LMP nvarchar(20),
                                                                                                                                                                                                                                           
@EDD nvarchar(20),
                                                                                                                                                                                                                                           
@SPo2 nvarchar(50),
                                                                                                                                                                                                                                          
@PROID int,
                                                                                                                                                                                                                                                  
@ExVU datetime,
                                                                                                                                                                                                                                              
@IsEmergency bit
                                                                                                                                                                                                                                             
AS
                                                                                                                                                                                                                                                           
Declare @TROPD bit
                                                                                                                                                                                                                                           
Select @TROPD=TROPD from tblUser Where UserId=@UserId
                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
Declare @SStatus bit
                                                                                                                                                                                                                                         
Declare @BDStatus bit
                                                                                                                                                                                                                                        
Select @SSTatus=ChangeSaved,@BDStatus=IsBackDate from tblMenuItems Where UserId=@UserId and SubMenuId=(Select Id from tblSubMenu Where frm_Code='frmOPD')
                                                                                                    
Declare @D int
                                                                                                                                                                                                                                               
Declare @OPDNoStart int
                                                                                                                                                                                                                                      
Declare @OPD int
                                                                                                                                                                                                                                             
Declare @UHIDType nvarchar(50) 
                                                                                                                                                                                                                              
Declare @ChangeValidUptoOnLastSlipVisit bit
                                                                                                                                                                                                                  
Select @ChangeValidUptoOnLastSlipVisit=Isnull(ChangeValidUptoOnLastSlipVisit,'False') from tblDoc Where DID=@CDID 
                                                                                                                                           
if exists (Select OPDDate from tblOPD Where OPDDate>@OPDDate and @BDStatus='True') Or not exists (Select OPDDate from tblOPD Where OPDDate>@OPDDate)
                                                                                                         
Begin
                                                                                                                                                                                                                                                        
if @OP=1
                                                                                                                                                                                                                                                     
begin
                                                                                                                                                                                                                                                        
Select @FileCharges=isnull(max(FileCharges),0)+1 from tblOPD Where OPDDate=@OPDDate
                                                                                                                                                                          
if @ValidUpto>=@OPDDate -- Valid Upto cannot be less then opddate
                                                                                                                                                                                            
Begin
                                                                                                                                                                                                                                                        
Select  @UHIDType=UHIDType from tblUser Where UserId=1 
                                                                                                                                                                                                      
if @UHIDType='Date Wise' -- jaroori h Del nahi krna date wise or sessionwise reg m uhid k 
                                                                                                                                                                   
Select @MembershipNo=isnull(max(MembershipNo),0)+1 from tblRegistration Where RegDate=@OPDDate
                                                                                                                                                               
else if @UHIDType='Session Wise' -- -- USE ONLY FOR REGISTRATION IN UHID GENRATE
                                                                                                                                                                             
Select @MembershipNo=isnull(max(MembershipNo),0)+1 from tblRegistration Where SessionId=@SessionId
                                                                                                                                                           

                                                                                                                                                                                                                                                             
if not exists (Select RegNo from tblRegistration Where RegNo=@RegNo)
                                                                                                                                                                                         
Begin
                                                                                                                                                                                                                                                        
EXEC CalculateRegNo @OPDDate, @SessionId, @RegNo OUTPUT;
                                                                                                                                                                                                     
End
                                                                                                                                                                                                                                                          
--For Generating OPD BillNo
                                                                                                                                                                                                                                  
EXEC GenOPDBillNo @OPDDate = @OPDDate, @Sessionid = @SessionId, @D = @D OUTPUT
                                                                                                                                                                               
--OPD No Doctor Wise Generate krne k liye
                                                                                                                                                                                                                    
EXEC GetOPDNo @DID = @CDID, @Date = @OPDDate, @Sessionid = @SessionId, @OPDNo = @OPD OUTPUT
                                                                                                                                                                  
if not Exists(Select SessionId from tblOPD Where SessionId>@SessionId)
                                                                                                                                                                                       
Begin
                                                                                                                                                                                                                                                        
if Not Exists(Select RegNo from tblRegistration where RegNo=@RegNo and SessionId=@SessionId)
                                                                                                                                                                 
insert into tblRegistration values(@RegNo,@OPDDate,@Name,@DOB,@Age,@AgeIn,@Sex,@Type,@FName,@MName,@ContactNo,@Address,@Weight,@WeightIn,'',@RDID,@Status,GetDate(),@UserId,@IsMember,@MemberShipNo,@MemberType,@SessionId,@PatientType,@SessionId,@City,@State
,@AadharNo)
                                                                                                                                                                                                                                                  
if Not Exists(Select RegNo,CDID,OPDDate,Sessionid,IsEmergency from tblOPD Where RegNo=@RegNo and CDID=@CDID and OPDDate=@OPDDate and SessionId=@SessionId and @IsEmergency=0)
                                                                                
Begin
                                                                                                                                                                                                                                                        
insert into tblOPD values(@D,@RegNo,@OPD,@OPDDate,@CDID,@OPDCharges,@Problem,@Status,@Date,@OPDType,'','','False',@Complaints,@FollowUp,'False',@UserId,@ValidUpto,@Note,@MemberShipNo,@SessionId,@Refund,@Reason,'False',@FileCharges,@Date,@SessionId,@FeeTyp
e,@BP,@PLUSE,@WEIGHT,@PaymentMode,@Balance,@OPDFee,@Online,@OPDDate,0,0,@TPAID,@Age,@BGS,@PType,@LMP,@EDD,@Temp,@Spo2,'False',@PROID,(CASE WHEN @ChangeValidUptoOnLastSlipVisit = 1 THEN @ExVU ELSE @ValidUpto END),@IsEmergency,@RDID,'False')
              
return 1
                                                                                                                                                                                                                                                     
End
                                                                                                                                                                                                                                                          
else
                                                                                                                                                                                                                                                         
return 6
                                                                                                                                                                                                                                                     
End
                                                                                                                                                                                                                                                          
else
                                                                                                                                                                                                                                                         
return 5 -- for opd not to enter in greater session 
                                                                                                                                                                                                         
End 
                                                                                                                                                                                                                                                         
else return 7
                                                                                                                                                                                                                                                
End
                                                                                                                                                                                                                                                          
else if @OP=2
                                                                                                                                                                                                                                                
begin
                                                                                                                                                                                                                                                        
if @TROPD='True'
                                                                                                                                                                                                                                             
Exec CheckLogs @RegNo=@RegNo,@Name=@Name,@FName=@FName,@Age=@Age,@AgeIn=@AgeIn,@Address=@Address,@ContactNo=@ContactNo,@Type=@Type,@UserId=@UserId,@OPDDate=@OPDDate,@voucher='OPD',@DocId=@CDID,@SrNo=@SrNo,@SessionId=@SessionId,@Refund=@Refund,@Reason=@Rea
son,@OPDCharges=@OPDCharges,@Online=@Online,@Balance=@Balance,@OPDFee=@OPDFee
                                                                                                                                                                                
if @SStatus='True'
                                                                                                                                                                                                                                           
Update tblOPD Set OPDFee=@OPDFee,OPDCharges=@OPDCharges,Online=@Online,Balance=@Balance,OPDNo=@OPDNo,CDID=@CDID,Problem=@Problem,OPDType=@OPDType,Status=@Status,Note=@Note,Refund=@Refund,Reason=@Reason,FileCharges=@FileCharges,FeeType=@FeeType,BP=@BP,PLUS
E=@PLUSE,WEIGHT=@WEIGHT,PaymentMode=@PaymentMode,TPAID=@TPAID,Age=@Age,BGS=@BGS,PType=@PType,LMP=@LMP,EDD=@EDD,[Temp]=@Temp,Spo2=@Spo2,PROID=@PROID,IsEmergency=@IsEmergency,RefId=@RDID where SrNo=@SrNo and SessionId=@SessionId 
                          
else
                                                                                                                                                                                                                                                         
Update tblOPD Set OPDNo=@OPDNo,CDID=@CDID,Problem=@Problem,Status=@Status,Note=@Note,Refund=@Refund,Reason=@Reason,FileCharges=@FileCharges,FeeType=@FeeType,BP=@BP,PLUSE=@PLUSE,WEIGHT=@WEIGHT,PaymentMode=@PaymentMode,TPAID=@TPAID,Age=@Age,BGS=@BGS,PType=@
PType,LMP=@LMP,EDD=@EDD,[Temp]=@Temp,Spo2=@Spo2,PROID=@PROID,RefId=@RDID where SrNo=@SrNo and SessionId=@SessionId
                                                                                                                                           
Update tblRegistration Set Name=@Name,DOB=@DOB,Sex=@Sex,Type=@Type,FName=@FName,MName=@MName,ContactNo=@ContactNo,Address=@Address,RDID=@RDID,Status=@Status,Date=GetDate(),IsMember=@IsMember,MemberType=@MemberType,PatientType=@PatientType,State=@State,Cit
y=@City,AadharNo=@AadharNo where RegNo=@RegNo
                                                                                                                                                                                                                
--This Is For Because Age Doesnt Effec Evry Time OPD AGe Because Billing Get Age from Registration
                                                                                                                                                           
Update tblRegistration Set Age=@Age,[Age In]=@AgeIn,Weight=@Weight,[Weight In]=@WeightIn where RegNo=@RegNo and SessionId=@SessionId
                                                                                                                         
end
                                                                                                                                                                                                                                                          
if @OP=3
                                                                                                                                                                                                                                                     
begin
                                                                                                                                                                                                                                                        
if not Exists(Select OPDDate from tblOPD WHere OPDDate>@OPDDate)
                                                                                                                                                                                             
Begin
                                                                                                                                                                                                                                                        
if @TROPD='True'
                                                                                                                                                                                                                                             
Begin
                                                                                                                                                                                                                                                        
Insert into tblLogs Values(@SrNo,@RegNo,@Name + ' ' + @Type + ' ' + @FName + ' Add = ' + @Address + ' Cont. = ' + @ContactNo + ' Fee = ' + Cast(@OPDCharges as nvarchar(50)) + Cast(@Online as nvarchar(50)) + ' Bal ' + Cast(@bALANCE as nvarchar(50)) ,@OPDDa
te,Getdate(),@UserId,'OPD Deleted')
                                                                                                                                                                                                                          
End
                                                                                                                                                                                                                                                          
Delete from tblOPD where SrNo=@SrNo and RegNo=@RegNo and OPDNo=@OPDNo and SessionId=@SessionId
                                                                                                                                                               
if Not Exists(Select RegNo from tblOPD where RegNo=@RegNo and SessionId=@SessionId) and Not Exists(Select RegNo from tblOPDetailMaster where RegNo=@RegNo and SessionId=@SessionId) and Not Exists(Select RegNo from tblIPD where RegNo=@RegNo and SessionId=@S
essionId) and Not Exists(Select UHID from tblProcedure where UHID=@RegNo and SessionId=@SessionId) and Not Exists(Select RegNo from tblLaboratoryDetailMaster where RegNo=@RegNo and SessionId=@SessionId) and Not Exists(Select RegNo from tblProcedureMaster 
where RegNo=@RegNo and SessionId=@SessionId)
                                                                                                                                                                                                                 
Delete from tblRegistration where RegNo=@RegNo and SessionId=@SessionId
                                                                                                                                                                                      
End
                                                                                                                                                                                                                                                          
else return 11
                                                                                                                                                                                                                                               
End
                                                                                                                                                                                                                                                          
if @OP=4
                                                                                                                                                                                                                                                     
Update tblOPD Set Refund=@Refund,Reason=@Reason Where SrNo=@SrNo and SessionId=@SessionId
                                                                                                                                                                    
End
                                                                                                                                                                                                                                                          
else 
                                                                                                                                                                                                                                                        
return 10 -- Back Date Entry Message
                                                                                                                                                                                                                         
