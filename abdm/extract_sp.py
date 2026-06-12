
import pyodbc

conn_str = 'DRIVER={SQL Server};SERVER=DESKTOP-JQ8GO5G\\SQLSTANDARD12;DATABASE=HMSS;UID=sa;PWD=heera11**'
conn = pyodbc.connect(conn_str)
cursor = conn.cursor()

cursor.execute("sp_helptext 'IUDOPD'")
rows = cursor.fetchall()

with open('IUDOPD_definition.sql', 'w') as f:
    for row in rows:
        f.write(row[0])

conn.close()
print("Stored procedure definition saved to IUDOPD_definition.sql")
