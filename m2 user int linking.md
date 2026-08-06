

|To<br>verif<br>y<br>when<br>invalid RequestID is<br>pass in header|<br>[<br>{<br> "key":"REQUEST-ID",<br> "value":"_{{$guid}}_zxzzxs",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1030: ",<br>"message":"Invalid requ est ID" <br>}<br> <br>Code:400Bad Request|
|---|---|---|
|When<br>Timestamp is<br>Blank, null or empty<br>in header.|[<br>{<br> "key":"TIMESTAMP",<br> "value":"",<br> "type":"text" <br>} ]|<br>Access Denied<br>Code :403Forbidden<br> <br> <br>|
|When invalid<br>Timestamp is pass in<br>header|[<br>{<br> "key":"TIMESTAMP",<br> "value":"_{{$isoTimestamp}}_jhg�ytgtyu",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1016: ",<br>"message":"Invalid Time stamp"<br>}<br> <br>Code -400Bad Request<br>|
|||<br>Access Denied<br>Code :403Forbidden<br>|
|When X-CM-ID<br>is Invalid, Blank, null<br>or empty in header.|[<br>{<br> "key":"X-CM-ID",<br> "value":"sbxdvdfvdf",<br> "type":"text" <br>}<br>]|Access Denied<br>Code :403Forbidden|
|When given HIP id<br>does not exist.||{<br> "code":"ABDM-1035: ", <br>"message":"Invalid HIP<br>ID" }<br>|



**Response Body:** 

60 





Callback API for SMS Notification to patients 

This is a callback API triggered by HIE-CM to notify HIP/HRP about SMS notification {callbackURL}/api/v3/patients/sms/on-notify 









|Description<br>Authorization<br>ABDM session API after successful<br>Validation of client id and secret<br>he actual time when the request was<br>initiated, ISO Date time format represents<br>he date and time|
|---|











# timestamp The actual UTC time when the request was initiated, ISO Date time format represents the date and time ~~<u>|| pan eff]a</u>~~ <u>Pf]</u> ~~<u>a</u>~~ 





<!-- Start of picture text -->
743ec386-670f-43a8-a3ed-44aa30fb15fb<br>timestamp<br>6f0b4665-a915-4c92-aa36-65afb4a2cd71<br><!-- End of picture text -->







<!-- Start of picture text -->
http/Awebhook site/b799c0b8-4e75-4545-Beb2-ddc2d5f0cSf6/api/v3/patients/s x-b3-sampled e<br>ms/on-notify x-b3-spanid cdidded72Feds42a<br>Host Eee $= VbI SSuodadh\Nemy: Cenk Valerie X-b3-traceid 15esee2edbb42691cdidded72Fed842a<br>Date 08/09/2024 8:10:35 PM (a few seconds ago) X-envoy-attempt-count 1<br>Size 132 bytes X-envoy-peer- sidecar~1@.223.109,22e~hiecm-hip-app-Sd87bb9b9d-tpgls.abdm2..<br>Time 0.000 sec metadata-id<br>ID 62154 ffb-e160-48ee-92f2-cOdbff446500 x-envoy-peer- ChoKCKNMVVNURVIFSUQSDBOKS3ViZXIUZXRIcwoOgCgXITINUQUSDRVSIUFM<br>Note @ Add Note metadata<br>x-requestid c2ed51d9-dfed-454d-a9ed-871987¢5143e<br>content-length 132<br>content-type application/json<br>x-hip-id Mohan_HIP<br>timestamp 2024-08-09714:4@:35.565Z<br>request-id 276fef41-9867-46de-bacs-bbecasdadcfe<br>authorization Bearer eyIhbGcioiIsuUzIiNiIsInRScCIgOiAisldULiwie21kIiAEIcze<br>accept +/*<br>user-agent Reactornetty/1.0.2¢<br>accept-encoding gzip<br>host webhook. site<br>Query strings Form values<br>(empty) (empty)<br>Raw Content [Format JSON EAword-wrap Copy<br>j<br>"requestId": null,<br>“timestamp”: null,<br>"status": "ACKNOWLEDGED",<br>“error": null,<br>Ac2el8c Of6/ani/u3 inationteme/on-nntity<br><!-- End of picture text -->

5.User Initiated Linking 

User-initiated linking is the process in which Users/Patient search for their health records from ABDM-compliant health facilities. Once health records are found, 

The user must have a Patient HIU (PHR App in the current scenario) via which the user 

Following are the steps involved in User initiated linking 



User makes a discovery request via Patient HIU (PHR App) —i.e., requests the HIP to 

The HRP/HIP is expected to search its database for any records that match this patient. 

The HRP/HIP will perform the validation by sending an OTP to the registered mobile. 

If the authentication succeeds, the care contexts are linked to the PHR address 



<!-- Start of picture text -->
Actor : '<br>' H ‘apw/hiecm/user-initiated-linking/v3 : '<br>H ‘ patient/care-context/discover '<br>aereturn {callback_url}//api/v2/nip/patient/care-context/discover— '<br>' return<br>' ; /apv/hiecm/user-initiated-linking/v3POST<br>' patient/care-context/on-discover<br>return<br>as<br>POST<br>{callback_url}/api/v3/hiu/patient/care-context/on-discover<br>return<br><!-- End of picture text -->

ABDM - HIE-CM - Discover Sequence Diagram 





<!-- Start of picture text -->
' ) [msn |<br>H Actor : '<br>' ‘ ‘ H<br>4 :‘ /apihieceVuser-initiated-linking/v3POST ' 'i<br>' fink/eare-context/init H<br>‘‘<br>'<br>H' eeereturn eee eee {callback _urlVapi/v3/hip/link/care-contextinitPOST :<br>return<br>'a<br>‘<br>‘H<br>H ‘<br>' : POST<br>' Japvhiecm/user-initiated-linking/v3<br>H Nink/care-context/on-init<br>‘<br>‘<br>‘<br>‘<br>‘ return<br>POST<br>{callback_urlWapi/v3/hiu/patient/care-context/on-init<br>return<br>ABDM - HIE-CM- Link Init Sequence Diagram<br>an7H'[won|r<br>‘HActor'‘/‘H ' ‘<br>H<br>H :: Japwhiecmuser-initiated-linking/v3FOST : 'H<br>'‘llink/care-context/confirm H‘<br>'<br>H Woo return eee POST :<br><!-- End of picture text -->

ABDM - HIE-CM - Link Confirm Sequence Diagram 



## Patient Health record discovery 

API will be invoked by the patient/user from the PHR application to HIECM to 

/api/hiecm/user-initiated-linking/v3/patient/care-context/discover 









|Description<br>Authorization<br>ABDM session API after successful<br>validation of client id and secret<br>The actual time when the request was<br>initiated, ISO Date time format<br>represents the date and time<br>JWT Authentication token which was<br>issued byABDM after<br>successful validation of username and|
|---|





|~~P}ooUL —~~<br>Description<br>~~Pf~~<br>~~UL~~<br>~~——~~<br>unverifiedidentifiers<br>Identifiers usingwhich the HIP will search<br>the patient information in their records.<br>“unverifiedidentifiers"|
|---|











|Scenarios|Headers/Body|Message|
|---|---|---|
|To verify when<br>Request ID is Blank,<br>null or empty in<br>header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"",<br> "type":"text" <br>} ]|Access Denied<br>Code :403Forbidden<br> <br>|
|To<br>verif<br>y<br>whe<br>n<br>invalid RequestID is<br>pass in header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"_{{$guid}}_zxzzxs",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1030: ",<br>"message":"Invalid reque st ID" <br>}<br> <br>Code:400Bad Request|
|When Timestamp<br>is Blank, null or<br>empty in header.|[<br>{<br> "key":"TIMESTAMP",<br> "value":"",<br> "type":"text" <br>} ]|<br>Access Denied<br>Code :403Forbidden<br> <br> <br>|
|When invalid<br>Timestamp is pass in<br>header|[<br>{<br> "key":"TIMESTAMP",<br> "value":"_{{$isoTimestamp}}_jhg�ytgtyu",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1016: ",<br>"message":"Invalid Times<br>tamp" <br>}<br> <br>Code -400Bad Request<br>|
|When X-HIP-ID is<br>Blank,<br>null<br>or<br>empty in header.|[<br>{<br> "key":"X-HIP-ID",<br> "value":"",<br> "type":"text" <br>} ]|<br> <br>Access Denied<br>Code :403Forbidden<br>|



68 



|When X-CM-ID<br>is Invalid, Blank, null|[<br>{|Access Denied|
|---|---|---|
|or empty in header.|"key":"X-CM-ID",<br> "value":"sbxdvdfvdf",|Code :403Forbidden|
||"type":"text"||
||}<br>]||
|When X-Auth-<br>TOKEN is Invalid in<br>header.|[<br>{<br> "key":"X-LINK-TOKEN",<br> "value":"hghhjjkhjkbkjbjkbkjbnkjbk",<br> "type":"text" <br>} ]|{<br> ""code"":""ABDM-1066:"",<br>""message"":""Invalid JWT token""<br>}<br> <br>Code -400Bad Request"|
|Verify when HIP is<br>null, blank or invalid<br>in the body|{<br> "hip": {<br> "id":"" <br>},<br> "unverifedIden�fers": [<br>{<br> "type":"MR",<br> "value":"69128688344" <br>}<br>] }|{<br> "code":"ABDM-9999: ",  <br>"message":"HIP ID is mandatory" <br>}|
|When X-HIU-ID and<br>the hipId in the<br>payload is same.|<br>|{<br>""code"": ""ABDM-1031<br>: "",<br>""message"": HIP and HIU<br>cannot be same""<br>}<br> <br>Code - 400Bad Request"|
|When<br>duplicate<br>request payload is<br>sent.|<br>|{<br>"code": "ABDM-1103: ",<br>""message"": “Duplicate<br>Discovery request“<br>}<br> <br>Code - 400Bad Request"|



69 



raised by the patient using HIE-CM’s discovery. 

{callback_url}/api/v3/hip/patient/care-context/discover 







|Description<br>The actual time when<br>initiated, ISO Date time<br>date and time<br>Identifier of the health<br>informationprovidedto|
|---|





#### Body Parameters: 

|ransactionld|03813343-ebc5-4c9f-<br>Yes<br>unique transaction for us¢|Transaction Id <br>r-initiated|i$ required to identifythe 89b6-6e9a75fd7c92<br>care context linking. This chains all the steps to<br>link care contexts. Transaction Id will be returned<br>after a successful discovery request to HIP by the<br>patient.|
|---|---|---|---|
|patient||Yes|A list of records ofthe patientthatwere<br>found as a result ofthe identifiers that the patient<br>had provided.|
|d<br>9162484106X|XXX@sbx<br>Yes<br>ABHA|Address ofthe pat|ient verifiedidentifiers<br>-<br>Yes|
||||he<br>Viopilie<br>number<br>ang Abha<br>mbe<br><br>|
|verifiedidentifiers- <br>ameUser1YesNam<br>"transaction|d"<br>"patient"|a<br>e ofthe patientGenderMYesG<br>R|ae<br>enderoftheuser<br>equest Body:|Theidentification detailsprovided by<br>he<br>patient<br>in<br>Discover<br>API<br>hod<br>Pe<br>Year ofBirth of<sup>the patient</sup>|



"verifiedidentifiers" 



"unverifiedidentifiers" 





<!-- Start of picture text -->
| POST | http://webhook site/b799c0b8-4e75-4545-Beb2-d8c2d5f0cSi6/api/va/hip/pati.. x-b3-sampled a<br>Host 14,143.232.14@. Whois Shodan Netify Censys VirusTotal x-b3-parentspanid 7dece34a3b689615<br>Date 08/09/2024 11:07:20 PM PM (5 minutes minutes ago) x-b3-spanid 2684cfigcledibfé<br>Size 368 bytes bytes x-b3-traceid 6326875836e9c f3b32225ab5efbe36e f3b32225ab5efbe36e<br>Time 0.000 sec sec x-envoy-attempt-count 1<br>ID Td7e7e45-3b97-40bc-ac6d-e40f1a44614F x-envoy-peer- sidecar~19.233.93,7@~abdm-hiecm-user-initiated-linking-api-...<br>Note i Add Note Add Note Note metadataid<br>X-envoy-peer- ChOKCKNMVVNURVI FSUQSDBOKS 3ViZX3 UZXR lcwoeCexXI TINUQUSDRVSIUFM. ..<br>metadata<br>x-request-id d3e25fe5-198f-4390-a4f4-99fadabassse<br>contentength 368<br>content-type application/json<br>x-hip-id Mohan_HiP<br>authorization Bearer eyIhbGciOiJsuzIINiIsInRScCIgOiAiS1dULiwia21kIiAsICIB...<br>timestamp 2@24-08-09T17:37:2@.621Z<br>request-id 4albfcéf-S5f1a-4252-8aFc-9032539637C8<br>accept aft<br>user-agent Reactornetty/1.0.24<br>host webhook, site<br>Query strings Form values<br>(empty) (empty)<br>Raw Content Content Format JSON JSON Word-Wrap Copy<br>{<br>“transactionId": “alc8f54e-4eab-4369-9cd9-ae@ebsG85724F",<br>"patient": {<br>“id": “vignesh_1992@sbx",<br>“verifiedidentifiers": [<br>t<br><!-- End of picture text -->

| POST | http://webhook site/b799c0b8-4e75-4545-Beb2-d8c2d5f0cSi6/api/va/hip/pati.. x-b3-sampled a Host 14,143.232.14@. Whois Shodan Netify Censys VirusTotal x-b3-parentspanid 7dece34a3b689615 Date 08/09/2024 11:07:20 PM PM (5 minutes minutes ago) x-b3-spanid 2684cfigcledibfé Size 368 bytes bytes x-b3-traceid 6326875836e9c f3b32225ab5efbe36e f3b32225ab5efbe36e Time 0.000 sec sec x-envoy-attempt-count 1 ID Td7e7e45-3b97-40bc-ac6d-e40f1a44614F x-envoy-peersidecar~19.233.93,7@~abdm-hiecm-user-initiated-linking-api-... Note i Add Note Add Note Note metadataid X-envoy-peerChOKCKNMVVNURVI FSUQSDBOKS 3ViZX3 UZXR lcwoeCexXI TINUQUSDRVSIUFM. .. metadata x-request-id d3e25fe5-198f-4390-a4f4-99fadabassse contentength 368 content-type application/json x-hip-id Mohan_HiP authorization Bearer eyIhbGciOiJsuzIINiIsInRScCIgOiAiS1dULiwia21kIiAsICIB... timestamp 2@24-08-09T17:37:2@.621Z request-id 4albfcéf-S5f1a-4252-8aFc-9032539637C8 accept aft user-agent Reactornetty/1.0.24 host webhook, site Query strings Form values (empty) (empty) Raw Content Content Format JSON JSON Word-Wrap Copy { “transactionId": “alc8f54e-4eab-4369-9cd9-ae@ebsG85724F", "patient": { “id": “vignesh_1992@sbx", “verifiedidentifiers": [ t 

HMIS/LIMS application 

|/api/hiecm/user-initiated-linking/v3/patient/care-context/on-discover<br>Description<br>Authorization<br>after successful validation of<br>~~Pp)~~<br>~~lddB~~|
|---|







|The actual time when the<br>request was initiated, ISO<br>Date time format represents<br>the date and time<br>Description<br>transactionld<br>Transaction Id is required to identifythe unique<br>transaction for user-initiated care context linking.<br>Transaction Id will be returned after a successful<br>discovery request to HIP bythe patient.<br>patient<br>Optional<br>A listofrecords ofthe patientthatwerefound as a<br>result ofthe identifiers thatthe patient had<br>Request ID sent bythe patient in the discovery API<br>of linking care contexts for a patient|
|---|









<!-- Start of picture text -->
Optional<br>or the patient at HIP for the given patient<br>message": "Patient not identifiers. The error should contain ABDM<br><!-- End of picture text -->



<!-- Start of picture text -->
"transactionid"<br>"patient"<br>"transaction|d"<br>"Patient not found"<br><!-- End of picture text -->



<!-- Start of picture text -->
"transactionid"<br>"patient"<br>"transaction|d"<br>"Patient not found"<br><!-- End of picture text -->



### **Response:** 

### Response 

Code: 202 Accepted 

### **Error Scenarios:** 







|**Scenarios**|**Headers/Body**|**Message**|
|---|---|---|
|To verify when Request ID is Blank,<br>null or empty in header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"",<br> "type":"text" <br>} ]|Access Denied<br>Code :403Forbidden|
|To verify when invalid<br>Request-ID is pass in header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"_{{$guid}}_zxzzxs",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1030: ",<br> "message":"Invalid request I D" <br>}<br>Code:400Bad Request|
|When Timestamp is Blank, null or<br>empty in header.|[<br>{<br> "key":"TIMESTAMP",<br> "value":"",  <br>"type":"text" <br>} ]|Access Denied<br>Code    :403Forbidden|
|When invalid Timestamp is pass in<br>header|[<br>{<br> "key":"TIMESTAMP",<br> "value":"_{{$isoTimestamp}_<br>_}_jhg�ytgtyu",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1016: ",  <br>"message":"Invalid Timesta mp" <br>}<br>Code      -400Bad Request|



75 



|"transactionld"<br>"patient"<br>"Invalid Transactio n ID /<br>Transaction expired."<br>Verifywhen transaction id is invalid,|
|---|





|Verify message when HI types is<br>passed as incorrect|"transac�onId":"776a9becab12-<br>42bc-9ae9c63b1ae5bce2",<br>"pa�ent": [<br>{<br> "referenceNumber":"ST1",<br> "display":"",<br> "careContexts": [<br>{<br> "referenceNumber":"S T2",<br> "display":"ST2" <br>}<br>],<br> "hiType":"PRESCRIPTION",<br> "count":1 <br>}<br>],<br>"matchedBy": [<br> "MR" <br>],<br>"response": {<br> "requestId":"6f37ddf8-<br>62df4afebc25-599789c90558" <br>|{<br> "code":"ABDM-9999: ",<br> "message":"Invalid HIType, it must<br>be in PRESCRIPTION, DIAGNOSTI<br>CREPORT,<br>OPCONSULTATION, DISCHARGES<br>UMMARY,<br>IMMUNIZATIONRECORD, HEALTH<br>DOCUMENTRECORD, WELLNESSR<br>ECORD" <br>}<br>400Bad Request<br>|
|---|---|---|
|Verify message when careconexts is<br>blank, null|"transac�onId":"776a9becab12-<br>42bc-9ae9c63b1ae5bce2",<br>"pa�ent": [<br>{<br> "referenceNumber":"ST1",<br> "display":"",<br> "careContexts": [<br>{<br> "referenceNumber":"S T2",<br> "display":"ST2" <br>}<br>],<br> "hiType":"PRESCRIPTION",<br> "count":1 <br>}<br>],<br>"matchedBy": [<br> "MR" <br>],<br>"response": {<br> "requestId":"6f37ddf8-62df-<br>4  afe-bc25-599789c90558"|{<br> "code":"ABDM-9999: ",<br> "message":"Invalid Care<br>Contexts count, must range be tween<br>1  to 20" <br>}<br>|



77 







<!-- Start of picture text -->
"transactionlid"<br>"patient"<br><!-- End of picture text -->

{callback_url}/api/v3/hiu/patient/care-context/on-discover 









|Description<br>The actual time when the request<br>was initiated, ISO Date time format<br>represents the date and time|
|---|





|~~P|~~<br>~~UL~~<br>~~——~~<br>Description<br>transactionld<br>Transaction Id is required to identify the unique<br>transaction for user-initiated care context linking.<br>Transaction Id will be returned after a successful<br>discovery request to HIP by the patient.<br>patient<br>A list of records ofthe patientthatwerefound as a<br>result ofthe identifiers that the patient had<br>~~Pf——~~<br>~~PTa~~<br>care contexts for a patient<br>477afb1865f6|
|---|







"transaction|d" "patient" 





"d89525a2-3a3f-4d39-98b5-477afb1865f6" 

| POST | http:/Awebhook site/b799c0b3-4e75-4545-Beb2-d8c2d5f0cSt6/api/v3/hiu/pati x-b3-sampled 8 Host 414.143.232.148 Whois Shodan Netify Censys VirusTotal x-b3-parentspanid S8bdeasssds3aa38 Date 08/09/2024 11:08:44 PM (14 minutes ago) x-b3-spanid 6695a4b437628ee5 Size 392 bytes x-b3traceid d56b5649927d26278013F9ac33e83e8s Time 0.001 sec xenvoy-attempt-count 1 ID eb49c749-56ac-4626-Sea6-e7b435ce2648 x-envoy-peersidecar~1@.233.85.12~abdm-hiecm-user-initiated-linking-api-... Note # Add Note scene x-envoy-peerChoKCKNMVVNURVIFSUQSDBOKS3ViZXIUZXR1CwoeCEXITINUQUSDRVSIUFM..metadata xaequestid 82c 20439 -O83f -457¢-a036-fS9eea73fSla content-length 392 content-type application/json x-hiu-id Mohan_HTU authorization Bearer eyJhbGcioijsuzIINilsInascCIgoiAiSldUliwia21kIiaeicia timestamp 2024-@8-89717:38142.4992 request-id 26403985 -717¢-4a58-be3d-dbd22scseb3t accept =f? user-agent Reactornetty/1.0.24 host webhook.site Query strings Form values (empty) (empty) Raw Content Format JSON [@Word-wrap Copy t “transactionId”: "alc8f54e-4eab-4369-9cd9-ab0b8G85724F", “patient”: [ ef “referenceNumber": "vignesh_1992@sbx", “display”: “88f54b1b-296f-4b62-9f50-b789229f2103", 





<!-- Start of picture text -->
¥ Request Content<br><!-- End of picture text -->

Patient health record link init 

This API will be invoked by the patient to link his/her health records. 

/api/hiecm/user-initiated-linking/v3/link/care-context/init 



<!-- Start of picture text -->
Authorizatio<br><!-- End of picture text -->







<!-- Start of picture text -->
Description<br>ABDM session API after successful<br>validation of client id and secret<br>Actual time when request was initiated,<br>ISO Date time format represents date and<br>time<br><!-- End of picture text -->



<!-- Start of picture text -->
Description<br>Authorizatio<br>ABDM session API after successful<br>validation of client id and secret<br>Actual time when request was initiated,<br>ISO Date time format represents date and<br>time<br><!-- End of picture text -->



|JWT Authentication token which was<br>issued byABDM after successful<br>validation of username and password<br>Identifier of the health information user<br>bywhich the request was initiated<br>Description<br>transactionld<br>Transaction Id is required to identify the unique<br>transaction for user-initiated care context linking. This<br>chains all the steps to link care contexts. Transaction Id<br>will be returned after successful discovery request to<br>HIP by the patient.<br>patient<br>A list of records ofthe patientthatwerefound as a<br>result ofthe identifiers that the patient had provided.|
|---|













<!-- Start of picture text -->
"transaction|d"<br>"patient"<br><!-- End of picture text -->







<!-- Start of picture text -->
"transactionld"<br>ransaction id is "Invalid Transaction ID"<br><!-- End of picture text -->



|Verify message<br>when count is<br>incorrect|"transac�onId":<br>"776a9becab1242bc9ae9c63b1ae5bce2",<br>"pa�ent": [<br>{<br> "referenceNumber":"Tes�ng defect",<br> "careContexts": [<br>{<br> "referenceNumber":"1234",<br> "display":"12" <br>} ],<br> "hiType":"PRESCRIPTION",<br> <br>"count": 0   }<br>|"{ <br>"code":"ABDM-1059: ",<br>"message":"Invalid Care Contexts count"<br>}"|
|---|---|---|
|To verify when<br>Request ID is<br>Blank, null or<br>empty in header|[<br>{<br>"key":"REQUEST-ID",   <br>"value":"",<br>"type": "text"<br>} ]|Access Denied<br>Code : 403 Forbidden|
|To verify when<br>invalid RequestID<br>is pass in header|[<br>{<br> "key":"REQUEST-ID", <br>"value":"{{$guid}}zxzzxs", <br>"type":"text" <br>} ]|{<br>"code":"ABDM-1030:",<br>"message":"Invalid request ID" <br>}<br> Code:   400Bad Request|
|When<br>Timestamp<br>is Blank, null or<br>empty in header.<br> <br> <br> <br> <br> <br>|[<br>{<br>"key":"TIMESTAMP", <br>"value":"", <br>"type":"text" <br>} ]|Access Denied<br>Code : 403 Forbidden|



85 



|When invalid<br>Timestamp is<br>pass in header|[<br>{<br>"key": "TIMESTAMP",<br>"value":"{{$isoTimestamp}}jhg�ytgtyu", <br>"type": "text" <br>} ]|{<br>"code": "ABDM-1016: ", <br>"message": "Invalid Timestamp" <br>}<br> Code    - 400Bad Request|
|---|---|---|
|When X-CM-<br>ID is Invalid,<br>Blank, null or<br>empty in header.|[<br>{<br>"key":"X-CM-ID", <br>"value": "sbxdvdfvdf", <br>"type":"text" <br>} ]|Access Denied<br> Code   : 403 Forbidden|
|When X-HIU-<br>ID is Blank, null<br>or empty in<br>header.<br>|[<br>{<br> "key":"X-HIU-ID", <br>"value":"", <br>"type": "text" <br>} ]|Access Denied<br>Code : 403 Forbidden<br>|
|When<br>XAUTHTOKEN is<br>Invalid, Blank, null<br>or empty in<br>header.<br> <br> <br> <br> <br> <br>|[<br>{<br>"key":"X-CM-ID", <br>"value": "sbxdvdfvdf", <br>"type": "text" <br>} ]|{<br>"code":"ABDM-1065: ", <br>"message":"Invalid X Auth token" <br>}<br>|



86 





<!-- Start of picture text -->
transactionid"<br>"patient"<br>"Testing defect"<br>after discovery Invalid Transaction<br>Transaction expired."<br><!-- End of picture text -->

This API will be invoked by the HIE-CM to initiate the linking of patient health records to 



<!-- Start of picture text -->
|<br><!-- End of picture text -->



<!-- Start of picture text -->
Description<br>The actual time when the<br>request was initiated, ISO<br>Date time format represents<br>the date and time<br>Identifier of the health<br>information provider to which<br><!-- End of picture text -->



<!-- Start of picture text -->
| Description<br>The actual time when the<br>request was initiated, ISO<br>Date time format represents<br>the date and time<br>Identifier of the health<br>information provider to which<br><!-- End of picture text -->



|Authorization<br>after successful validation of<br>Description<br>ransactionld<br>Transaction Id is required to identify the unique<br>transaction for user-initiated care context linking.<br>Transaction Id will be returned after a successful<br>discovery request to HIP by the patient.<br>~~|~~<br>~~|~~<br>~~foroo~~<br>patient<br>A list of records ofthe patient that werefound as<br>a result ofthe identifiers thatthe patient had|
|---|











<!-- Start of picture text -->
"transaction|d"<br>"patient"<br><!-- End of picture text -->





<!-- Start of picture text -->
http:/webhook.site/b799c0b8-4e75-4545-Beb2-d8c2d5f0c9f6/api/va/hipilink/ca x-b3-sampled c}<br>tee erik x-b3-parentspanid as712esaleffcedé<br>Host eer =O (St em ne! x-b3-spanid 188¢31b2d3e87931<br>Date 0 8/ 09/.09/2024 11:09:33 PNPM (13 minut e ss ago)age x-b3-traceidi S2d F aee22esbdseesafscrissFseodas1€ f<br>Sizeize 2500 bytesbytes x-envoy-attempt-count 1<br>Time; 09.000 sec x-envoy-peer- sidecar~10, 233.93. 7@~abdm-hiecm-user-initiated-linking-api-<br>ID 12efd017-251a-4428-b8ef-b70adctab7fS metadata-id<br>Note if Add Note x-envoy-peer- ChoKCkNMVWNURVIFSUQSDBOKS3VAZXIUZXR LcwoeCgxITINUQUSDRVOIUFM...<br>metadata<br>x-request-id aeceseb3 -d3dF-4265-9630-20a1765f546e<br>content-length 25¢<br>content-type application/json<br>x-hip-id Mohan_HIP<br>authorization Bearer eyJhoGciOlISUZIANIIsInRScCIgOiAiSlduliwia21kIiAGIcyB...<br>timestamp 2024-08-89T17:39:33.1022<br>request-id 9bSfed18-6122-408d-beca-b3csba73eF7a<br>accept +/$<br>user-agent ReactorNetty/1.@.24<br>host webhook. site<br>Query strings Form values<br>(empty) (empty<br>Raw Content Format JSON Word-Wrap Copy<br>“transactionId": "alc8f54e-4eab-4369-9cd9-aGGbsGa5724F",<br>“abhadddress": “vignesh_1992@sbx",<br>“patient™: [<br>it<br>“referenceNumber": “vignesh_1992@sbx",<br><!-- End of picture text -->



<!-- Start of picture text -->
i ~ Request Details & Headers<br>(Fost) https://webhook site/7 119136d-2ea8-4681-acbb-7 150231 3ebf1/api/v3hhip/link/care-context/init contenttength 148<br>Host 14.143.232.148 Whois Shodan Netify Censys VirusTotal content-type application/json<br>Date 04/04/2025 11:40:08 AM (6 minutes ago) x-b3-sampled e<br>Size 140 bytes x-b3-parentspanid gecbefaescd91524<br>Time 0.000 sec x-b3-spanid balaedcf23e1b43b<br>ID b6b9b442-41c2-4eb8-8130-6db133a39281 x-b3-traceid c678S¥c11683c1d4e7a12deacfe79044<br>Note # Add Note xhip-id mAvUR_HIP<br>authorization Bearer ey3hbGciOiISUZTINiTsINRScCTgOiAISdUTiwia21kTiA6ICIabFIiNVdDbThUbTLFSI9IzkaSejAZaj<br>timestamp 2025-94-047@6:10:08.213Z<br>request-id 2b9acala-5385-4878-a4a3-e49bc15d9373<br>accept +/*<br>user-agent Reactornetty/1.0.24<br>Query strings Form values<br>~ Request Content<br>Raw Content @ Format JSON MiWword-Wrap Copy<br>“transactionId": "d682f433-3745-4175-9ed5-b5a14487bbe2",<br>“error”: {<br>“code”: "“ABDM-1056",<br>“message”: “This care contexts has been already linked"<br><!-- End of picture text -->











|Description<br>Authorization<br>issued byABDM session API after<br>~~fT~~<br>~~Pf~~<br>~~LE—~~<br>Actual time when request was<br>initiated, ISO Date time format<br>represents date and time|
|---|
||<br>Description<br>ransactionld<br>Transaction Id is required to identifythe<br>unique transaction for user initiated<br>steps to link care contexts.Transaction Id<br>will be returned after successful<br>discovery request to HIP by the patient.|







<!-- Start of picture text -->
records has to be linked to the patient’s<br>"authenticationType": "MEDIATE", authenticationType and the meta details<br>"communicationMedium": "MOBILE",<br>"communicationHint": "OTP",<br>"“communicationExpiry": "2023-<br>2b835afb-0c97-4ce7-9dd9ef58ee98a326<br>patient<br><!-- End of picture text -->





<!-- Start of picture text -->
"authenticationType"<br>"communicationMedium"<br>"communicationHint"<br>"communicationExpiry"<br>"2b835afb-0c97-4ce7-9dd9-ef58ee98a326"<br><!-- End of picture text -->









|Verifywhen transaction id<br>"transactionld"<br>"Invalid Transaction<br>"Testing defe ct"<br>"authenticationType"<br>"communicationMedium"<br>"communicationHint"<br>"communicationExpiry"<br>communication<br>"'transactionId"":<br>"Invalid communica tion<br>Testing defe ct<br>""authenticationType"™": ""<br>"'communicationMedium"":<br>"communicationHint"":<br>""'communicationExpiry"":|
|---|





|"'transactionid"™":<br>"Testing defe ct'"<br>""authenticationType"":<br>"'communicationMedium"":<br>"communicationHint"":<br>""'communicationExpiry"":<br>"transactionld":<br>"authenticationType":<br>"communicationMedium":<br>"communicationHint":<br>"communicationExpiry":|
|---|





|authenticationTypeis null<br>"transactionld":<br>“Invalid authentication type”<br>"authenticationType":<br>"communicationMedium":<br>"communicationHint":<br>"communicationExpiry":<br>communicationMedi um is<br>"transactionld":<br>communication medium”<br>"authenticationType": "",<br>"communicationMedium":<br>"communicationHint":<br>"communicationExpiry|
|---|





|communicationHint is null<br>"transactionld":<br>“Invalid communication hint”<br>"authenticationType":<br>"communicationMedium":<br>"communicationHint":<br>"communicationExpiry":|
|---|



{callback_url}/api/v3/hiu/patient/care-context/on-init 









<!-- Start of picture text -->
Description<br><!-- End of picture text -->



|Authorization<br>after successful validation of<br>~~Pf~~<br>oS<br>Actual time when request was<br>initiated, ISO Date time format<br>represents date and time<br>Identifier of the health<br>information user to which the<br>Description<br>|<br>transactionld<br>Transaction Id is required to identifythe<br>unique transaction for user initiated care<br>link care contexts.Transaction Id will be<br>returned after successful discovery request<br>to HIP by the patient.|
|---|





|records have to be linked to the patient’s<br>"authenticationType": "<br>authenticationTypeandthemeta detailsof<br>"communicationMedium": "MOBILE",<br>"communicationHint": "OTP",<br>“communicationExpiry": "2023-<br>2b835afb-0c97-4ce7-9dd9ef58ee98a326<br>care contexts for a patient|
|---|





<!-- Start of picture text -->
"transactionld"<br>"“authenticationType"<br>"communicationMedium"<br>"communicationHint"<br>"communicationExpiry"<br>"14a298b4-cf4a-497b-b4c1-72b50295fb91"<br><!-- End of picture text -->







<!-- Start of picture text -->
| Post | http://webhook site/b799c0b8-4e75-4545-Beb2-d8c2d5f0c9f6/api/v3/hiu/patien x-b3-sampled c)<br>Host Ycare-context/on-init x-b3-parentspanid 7baecf39s26110dF<br>Date 14,143.232.148 Whois Shodan Netify Censys VirusTotal x-b3-spanid 5¢caea779231e5"4<br>Size 33198/09/2024bytes  11:10:15 PM (13 minutes ago) x-envoy-attempt-countx-b3-traceid 112024b22385263b9sb2919882Fe1a0bd<br>Time 0.000 sec x-envoy-peer- sidecar~10,233.93.70~abdm-hiecm-user-initiated-linking-api-...<br>ID fef653f0-1d5e-4c84-8a65-44eeb3fbe245 metadata-id<br>Note @ Add Note X-envoy-peer- ChoKCKNMVVNURVIfSUQSDBOKS3ViZXIUZXR1 cwoeCEXITINUQUSDRVSIUFM...<br>metadata<br>x-request-id bb29b55d-6b38-4cdb-96ba-827965670c3¢<br>contentlength 331<br>Content-type application/json<br>x-hiu-id Mohan_HIU<br>authorization Bearer eyJhbGcioiIsuzIINiIsInRScCIgOiaislduIiwia21kTiasIcie...<br>timestamp 2024-@8-89T17;48:13.3222<br>request-id 46e7b826-6e32-4f75-bad9-Scdceesfices<br>accept s/s<br>user-agent Reactornetty/1.0.24<br>host webhook, site<br>Query strings Form values<br>{empty} (empty)<br>Raw Content Format JSON Word-Wrap Copy<br>{<br>“transactioniId"; "alc8f54e-4eab-4369-9cd9-aG0b8085724F",<br>“Link": {<br>“referenceNumber": “a9f3db7a-cOc2-4570-93be-89d2d0366533",<br>“authenticationType": "DIRECT",<br>“meta”: {<br><!-- End of picture text -->

patient 

/api/hiecm/user-initiated-linking/v3/link/care-context/confirm 



<!-- Start of picture text -->
Authorization<br><!-- End of picture text -->







<!-- Start of picture text -->
Description<br>after successful validation of<br><!-- End of picture text -->



<!-- Start of picture text -->
Description<br>Authorization<br>after successful validation of<br><!-- End of picture text -->





<!-- Start of picture text -->
Pf | ——<br>Actual time when request was<br>initiated, ISO Date time format<br>represents date and time<br>JWT Authentication token<br>after successful validation of<br>Identifier of the health<br>information user by<br>initiated<br>Description<br>initiating the linking of health<br>records of the patient<br><!-- End of picture text -->





"linkRefNumber": "4336268d-89a3-4c84-8674-aef42092d9fc" } 

### **Response:** 

Response Code : 202 Accepted 



### **<u>Error scenarios:</u>** 



<!-- Start of picture text -->
Scenarios<br>To verify when<br>Request ID is<br>Blank, null or<br>empty in<br>header<br><!-- End of picture text -->





<!-- Start of picture text -->
Message ge e<br><!-- End of picture text -->



<!-- Start of picture text -->
Scenarios  Headers/Body Message ge e<br>To verify when  [  Access Denied<br>Request ID is      {  Code : 403<br>Blank, null or  "key": "REQUEST-ID",  Forbidden<br>empty in  "value": "",<br>header  "type": "text"<br>    }  ]<br>To verify when  [  {<br>invalid RequestID      {  "code": "ABDM-1030: ",     "message":<br>is pass in header   "key": "REQUEST-ID",  "Invalid<br>"value": " {{$guid}} zxzzxs",  request ID"<br>"type": "text" }<br>    }<br>]<br> Code: 400Bad Request<br>When  [  Access Denied<br>Timestamp is      {  Code : 403 Forbidden<br>Blank, null or  "key": "TIMESTAMP",<br>empty in header.  "value": "",<br>"type": "text"<br>    }  ]<br><!-- End of picture text -->

101 



|When invalid<br>Timestamp is<br>pass in header|[<br>{<br> "key":"TIMESTAMP",<br> "value":"_{{$isoTimestamp}}_jhg�ytgtyu",<br> "type":"text" <br>} ]|{<br> "code":"ABDM-1016: ",<br> "message":"Invalid Timestamp" <br>}<br> <br>Code -400Bad Request<br>|
|---|---|---|
|When X- HIU-ID<br>is Blank, null or<br>empty in header.|[<br>{<br> "key":"X-HIU-ID",<br> "value":"",<br> "type":"text" <br>}<br>]|Access Denied<br>Code :403Forbidden|
|When X-CM-<br>ID is Invalid,<br>Blank, null or<br>empty in header.<br>|[<br>{<br> "key":"X-CM-ID",<br> "value":"sbxdvdfvdf",<br> "type":"text" <br>}<br>]|Access Denied<br>Code :403Forbidden|
|When<br>XAuthTOKEN is<br>Invalid in<br>header.|[<br>{<br> "key":"X-LINK-TOKEN",<br> "value":"hghhjjkhjkbkjbjkbkjbnkjbk",<br> "type":"text" <br>}<br>]|{<br> ""code"":""ABDM-1066:"",<br> ""message"":""Invalid<br>JWT token"" <br>}<br> <br> Code<br>-400Bad Request"|
|Verify<br>message when<br>invalid token is<br>passed|{<br> "token":7897654,<br> "linkRefNumber":"Tes�ng defect" <br>}|{<br> "code":"ABDM-9999: ","message":<br>Invalid link reference number." <br>}|
|Verify<br>message when the<br>X-<br>HIU-ID<br>is<br>diferent from the<br>hiu that ini�ated<br>link request.||{<br>"code": "ABDM-1040: ",<br>"message": “Invalid HIU ID."<br>}|



102 



HIE-CM callback for health record confirmation 

to confirm patient health records to HIP 









||<br>Description<br>|<br>Actual time when request was<br>initiated, ISO Date time format<br>represents date and time<br>Identifier of the health<br>information provider to which the|
|---|
|Description<br>while initiating the linking of<br>health records ofthe patient|









<!-- Start of picture text -->
| POST | http://webhook.site/b799c0b8-4e75-4545-Seb2-d8c2d5f0c9f6/api/v3/hipllink/ca x-b3-sampled 8<br>Host re-context/confirm x-b3-parentspanid #7203013 F2cb744<br>Date 08/09/202414,143.232.14811:10:49Whois PM (13Shodan minutesWNefify ago) Censys VirusTota' x-b 3-trace3-span id b7iich¢bdsf5¥e585¢49429bdbbf3 c bassbes2a2b3dac2<br>Size 124 bytes x-envoy-attempt-count 1<br>Te aogiisec x-envoy-peer- sidecar~1¢.233.93.7@~abdm-hiecm-user-initiated-linking-api-...<br>ID 56d79de9-e6d3-4f1b-88b4-98286d1d209 metadata-id<br>Note ff Add Note x-envoy-peer- ChoKCKNMVVNURW3 #SUQSOBOKS3ViZXIUZXR LewoeCeXITINUQUSDRVSIUFH<br>metadata<br>x-requestid 2d376d67-267d-4de0-sda9-basseaceaese<br>content-length 124<br>content-iype application/json<br>x-hip-id Mohan_HIP<br>authorization Bearer eyIhbGcioijsuzIinilsinrAscCIgoiAislduliwia2ikIiaeicia<br>timestamp 2024-8-9T17:40:48.9497<br>request-id 9ecc6aa6 -84b2-4089-b5c6-373F62d a scc<br>accept +;<br>user-agent ReactorNetty/1.@.24<br>host webhook. site<br>Query strings Form values<br>(empty) (empty)<br>Raw Content Format JSON E@Word-Wrap Copy<br>{<br>“requestid": null,<br>"timestamp": null,<br>“confirmation™: {<br>"token": "123456",<br>“LlinkRefNumber": "a9f3db7a-c0c2-4570-93be-89d2d0366533"<br><!-- End of picture text -->



<!-- Start of picture text -->
to share the response of link confirmation API<br>/api/hiecm/user-initiated-linking/v3/link/care-context/on-confirm<br>Description<br>Authorization<br>by ABDM session API after<br>successful validation of client id and<br>Pf  end request transaction<br><!-- End of picture text -->



|Actual time when request was<br>initiated, ISO Date time format<br>representsdateandtime|
|---|











|Description<br>|<br>patient<br>A list of records ofthe patient that werefound as<br>a result ofthe identifiers thatthe patient had<br>Link reference number used while initiating the<br>linking of health records ofthe patient|
|---|





"patient" 



"response": { 

"requestId": "f207e461-1994-4274-9b86-554384f170ab" 

} } 

### **Response:** 

Response 

Code : 202 Accepted 

### **Error scenarios:** 







|**Scenario s**|**Request Body**|**Response**|
|---|---|---|
|To verify<br>when<br>Request ID is<br>Blank, null or<br>empty in<br>header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"",<br> "type":"text" <br>} ]|Access Denied<br>Code :403Forbidden|
|To verify<br>when<br>invalid<br>Request-ID is<br>pass in header|[<br>{<br> "key":"REQUEST-ID",<br> "value":"_{{$guid}}_zxzzxs",  <br>"type":"text" <br>}]|{<br> "code":"ABDM-1030: ",<br> "message":"Invalid request ID" <br>}<br>Code:400Bad Request<br>|
|When X- HIP-ID<br>is Blank, null or<br>empty in header.|<br> <br>[<br>{<br> "key":"X-HIP-ID",<br> "value":"",  <br>"type":"text" <br>} ]|Access Denied<br>Code :403Forbidden<br>|



106 





|"patient"<br>"Testing defe ct"<br>"patient"<br>"Testingdefect"|
|---|





|{callback_url}/api/v3/hiu/patient/care-context/on-confirm<br>Description<br>he actual time when the request<br>was initiated, ISO Date time format<br>represents the date and time<br>~~Pp]~~<br>~~| —~~<br>Description<br>patient<br>A listofrecords ofthe patientthatwerefound as<br>a result ofthe identifiers that the patient had<br>Link reference number used while initiating the<br>linking of health records ofthe patient|
|---|











|requestId|f207e461-1994-<br>42749b86554384f170ab|Yes|Request ID sent in the init API call. This request ID<br>will be used to match the fow of linking care<br>contexts for a pa�ent|
|---|---|---|---|



### **Request Body:** 





<!-- Start of picture text -->
Request Body<br>{<br>    "pa�ent": [<br>        {<br>            "referenceNumber": "4336268d-89a3-4c84-8674-aef42092d9fc",             "display":<br>"abcdefgdisplay",<br>            "careContexts": [<br>                {<br>                    "referenceNumber": "1234",<br>                    "display": "1234-display"<br>                }<br>            ],<br>            "hiType": "PRESCRIPTION",<br>           "count": 1<br>        }<br>    ],<br>    "response": {<br>        "requestId": "f207e461-1994-4274-9b86-554384f170ab"<br>    }<br>}<br><!-- End of picture text -->

### **Response:** 

<mark>Response</mark> Code : 202 Accepted 

109 





<!-- Start of picture text -->
‘Post | http://webhook site/b799c0b8-4e75-4545-Beb2-d8c2d5f0c9i6/api/vI/hiu/patien x-b3-sampled 8<br>Host Ucare-context/on-confirm14.143.232.140 Whois Shodan Nelify Censys VirusTota! x-b3-parentspanidx-b3-spanid cadee14952e14225[CSS<br>Date 08/09/2024 11:12:00 PM (13 minutes ago) x-b3-traceid e64cfi1c2c4fe428ee8177576¢812e93<br>Size 298 bytes x-envoy-attempt-count 1<br>Time 0.001 sec X-envoy-peer- Sidecar~10.233.85.12~abdm-hiecm-user-initiated-linking-api-<br>ID de5b906b-ca76-4320-9da2-d50ef4b05748 metadata-id<br>Note # Add Note x-envoy-peer- ChoKCkNMVVNURVIFSUQSOBOKS3ViZXIUZXR1cwoeCgxI TINUQUSDRVSIUFM.<br>metadata<br>x-request-id dédc7ef9-e2f1-4a7d-a013-scbiebfssa7e<br>contenttength 298<br>content-type application/json<br>x-hiu-id Mohan_HIU<br>authorization Bearer eyJhbGciOiJSUZIINiIsINRScCIGOIAIS1dUIiwia21kIiAGICIB...<br>timestamp 2024-@8-99T17:41:58.5937<br>request-id addda9e3 -3380-4c9a-bfae-ffidifzedbfi<br>accept nied<br>user-agent Reactornetty/1.0.24<br>host webhook. site<br>Query strings Form values<br>(empty) (empty)<br>Raw Content [Format JSON EAWord-Wrap Copy<br>{<br>“patient™: [<br>{<br>“referenceNumber": “vignesh_1992@sbx",<br>"display": “88f54b1b-296f-4b62-9f50-b789229f2103",<br>“careContexts": [<br><!-- End of picture text -->

> a“Granted”)” and same .is notified+ £- to HIP and HIU. HIU sends pushback URL to HIP via HIECM. HIP now bundles the care context or Health data of the patient as per FHIR standards and share the data via pushback data URL. HIECM is notified 

