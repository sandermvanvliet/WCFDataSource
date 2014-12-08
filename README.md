WCF Data Source
===

A SQL Server Reporting Services data source for WCF services.

This Data Processing Extension (DPE) adds functionality to SSRS to consume WCF services a little bit easier that the currently existing XML datasource.  
When you specify the URL and operation to use the extension will automatically determine the parameters for the operation and return the list of fields in the result. If the operation returns an array or collection it will return the fields of the entity in the collection.

Usage
===


A word of warning
===
**No. 1:** I haven't got around to creating an install script to deploy the extension so at the moment you will need to do this manually. You will need to deploy the extension to the reporting server itself and your local Visual Studio directory.  
**No. 2:** Installing this extension will break your upgrade path. If you want to upgrade your Reporting Services you will need to remove the extension first.

**Deploying to SQL Server Reporting Services**
* Navigate to  
  C:\Program Files\Microsoft SQL Server\MSRS11.MSSQLSERVER\Reporting Services\ReportServer
* Create a back-up copy of rsreportserver.config and rssrvpolicy.config
* Copy the SanderVanVliet.WcfDataSource.dll to the bin directory
* Open the rsreportserver.config file and locate the `<Data>` section
* Add the following line:  
```xml

    <Extension Name="WCF" Type="SanderVanVliet.WcfDataSource.WcfConnection, SanderVanVliet.WcfDataSource" />  
```
* Open the rssrvpolicy.config and locate the `<CodeGroup>` element that refers to the Url **$CodeGen$**
* Below that `<CodeGroup>` add the following:
```xml

	<CodeGroup class="UnionCodeGroup"
		version="1"
		PermissionSetName="FullTrust"
		Name="WCF"
		Description="Full trust grant for the WCF data provider">
		<IMembershipCondition
			class="UrlMembershipCondition"
   			version="1"
			Url="file:///C:/Program Files/Microsoft SQL Server/MSRS11.MSSQLSERVER/Reporting Services/ReportServer/bin/SanderVanVliet.WcfDataSource.dll"
		/>
	</CodeGroup>
```

**Deploying to Visual Studio**
* Navigate to  
  C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\PrivateAssemblies
* Create a back-up copy of RSReportDesigner.config
* Copy the SanderVanVliet.WcfDataSource.dll to the PrivateAssemblies directory
* Open the rsreportserver.config file and locate the `<Data>` section
* Add the following line:  
```xml

    <Extension Name="WCF" Type="SanderVanVliet.WcfDataSource.WcfConnection, SanderVanVliet.WcfDataSource" />  
```
* Restart Visual Studio