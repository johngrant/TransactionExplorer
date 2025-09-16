An official website of the U.S. government Here's how you know

[Home](https://fiscaldata.treasury.gov/) / API Documentation

# API Documentation

# **Getting Started**

The U.S. Department of the Treasury is building a suite of open-source tools to deliver standardized information about federal finances to the public. We are working to centralize publicly available financial data, and this website will include datasets from the Fiscal Service on topics including debt, revenue, spending, interest rates, and savings bonds.

Our API is based on Representational State Transfer, otherwise known as a RESTful API. Our API accepts GET requests, returns JSON responses, and uses standard HTTP response codes. Each endpoint on this site is accessible through unique URLs that respond with data values and metadata from a single database table.

### **What is an API?**

API stands for **Application Programming Interface**. APIs make it easy for computer programs to request and receive information in a useable format.

If you're looking for federal financial data that's designed to be read by humans rather than computers, head to our [website](https://fiscaldata.treasury.gov/) to search for data (available in CSV, JSON, and XML formats) or visit our partner site, [USAspending](https://www.usaspending.gov/) ‒ the official source for spending data for the U.S. Government. There, you can follow the money from congressional appropriations to federal agencies down to local communities and businesses. For more general information, visit Your Guide to [America's](https://fiscaldata.treasury.gov/americas-finance-guide/) Finances, where Fiscal Data breaks down complex government finance concepts into easy-to-understand terms.

## **What is a Dataset?**

We present data to you in collections called datasets. We define a dataset as a group of data that has historically been published together as one report. In some cases, datasets consist of multiple tables, which correspond to sections of reports. When this is the case, datasets are powered by more than one API. For example, the Monthly Treasury Statement (MTS) dataset contains multiple APIs, corresponding with information on federal government spending, revenue, debt, and more.

[Search](https://fiscaldata.treasury.gov/datasets/) and filter our datasets to explore more.

### **API Endpoint URL Structure**

For simplicity and consistency, endpoint URLs are formatted with all lower case characters. Underscores are used as word separators. Endpoints use names in singular case.

The components that make up a **full API request** are below.

Base URL + Endpoint + Parameters and Filters (optional)

BASE URL EXAMPLE:

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/

ENDPOINT EXAMPLE:

v1/accounting/od/rates\_of\_exchange

PARAMETERS AND FILTERS EXAMPLE:

?fields=country\_currency\_desc,exchange\_rate,record\_date&filter=record\_da te:gte:2015-01-01

FULL API REQUEST EXAMPLE:

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/accou nting/od/rates\_of\_exchange?fields=country\_currency\_desc,exchange\_rate,re cord\_date&filter=record\_date:gte:2015-01-01

### **How to Access our API**

Our API is open, meaning that it does not require a user account or registration for a token. To begin using our API, you can type the GET, R, or Python request below directly into a web browser (or script in a data analysis tool), which will return a JSON-formatted response. You can also request CSV- or XML-formatted data by using the format filter.

EXAMPLE API REQUEST USING GET:

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/accou nting/od/rates\_of\_exchange?fields=country\_currency\_desc,exchange\_rate,re cord\_date&filter=country\_currency\_desc:in:(Canada-Dollar,Mexico-Peso),re cord\_date:gte:2024-01-01

EXAMPLE RESPONSE:

```
{"data":[{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.426","record_date":"2020-03-31"},
{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.26","record_date":"2021-03-31"},
{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.275","record_date":"2020-12-31"},
{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.368","record_date":"2020-06-30"},
{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.239","record_date":"2021-06-30"},
{"country_currency_desc":"Canada-Dollar",
"exchange_rate":"1.338","record_date":"2020-09-30"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"19.913","record_date":"2020-12-31"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"23.791","record_date":"2020-03-31"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"23.164","record_date":"2020-06-30"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"20.067","record_date":"2020-09-30"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"20.518","record_date":"2021-03-31"},
{"country_currency_desc":"Mexico-Peso",
"exchange_rate":"19.838","record_date":"2021-06-30"}],
"meta":{"count":12,"labels":{
"country_currency_desc":"Country-CurrencyDescription",
"exchange_rate":"ExchangeRate","record_date":"RecordDate"},
"dataTypes":{"country_currency_desc":"STRING","exchange_rate":"NUMBER",
"record_date":"DATE"},
"dataFormats":{"country_currency_desc":"String","exchange_rate":"10.2",
"record_date":"YYYY-MM-DD"},
"total-count":12,"total-pages":1},
"links":{"self":"&page%5Bnumber%5D=1&page%5Bsize%5D=100",
"first":"&page%5Bnumber%5D=1&page%5Bsize%5D=100","prev":null,
"next":null,"last":"&page%5Bnumber%5D=1&page%5Bsize%5D=100"}}
```

EXAMPLE API REQUEST USING R:

```
library(httr)
library(jsonlite)
request="https://api.fiscaldata.treasury.gov/services/api/fiscal_servic
e/v1/accounting/mts/mts_table_9?filter=line_code_nbr:eq:120&sort=-record
_date&page[size]=1"
response=GET(request)
out=fromJSON(rawToChar(response$content))
data=out$data
head(data)
EXAMPLE API REQUEST USING PYTHON:
# MTS Table 1 API - JSON Format
# Import necessary packages
import requests
import pandas as pd
# Create API variables
baseUrl = 'https://api.fiscaldata.treasury.gov/services/api/fiscal_servi
ce'
endpoint = '/v1/accounting/mts/mts_table_1'
fields = '?fields=record_date,parent_id,classification_id,classification
_desc,current_month_gross_rcpt_amt'
filter = '&filter=record_date:eq:2023-05-31'
```

sort = '&sort=-record\_date'

format = '&format=json'

pagination = '&page[number]=1&page[size]=3' API = f'{baseUrl}{endpoint}{fields}{filter}{sort}{format}{pagination}' # Call API and load into a pandas dataframe

data = requests.get(API).json() pd.DataFrame(data['data'])

### **License and Authorization**

The U.S. Department of the Treasury, Bureau of the Fiscal Service is committed to providing open data as part of its mission to promote the financial integrity and operational efficiency of the federal government. The data is offered free, without restriction, and available to copy, adapt, redistribute, or otherwise use for non-commercial or commercial purposes.

## **API Versioning**

Our APIs are currently in v1.0.0 or v2.0.0. To determine which version the API is in, please refer to the specific dataset detail page and navigate to the API Quick Guide > Endpoint section.

# **Endpoints**

Many datasets are associated with only one data table, and thus, one API endpoint. There are some datasets comprised of more than one data table, and therefore have more than one endpoint.

## **List of Endpoints**

The table below **lists the available endpoints by dataset and data table, along with a brief description** of the corresponding endpoint.

Note that every API URL begins with the base URL:

#### https://api.fiscaldata.treasury.gov/services/api/fiscal\_service

Thus, the full API request URL would be the Base URL + Endpoint. For example:

#### https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v2/accou nting/od/avg\_interest\_rates

<span id="page-4-0"></span>

| Dataset                                                              | Table<br>Name                                                        | Endpoint                          | Endpoint<br>Description                                                                                                                                                                                                                                                                                                                                            |
|----------------------------------------------------------------------|----------------------------------------------------------------------|-----------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 120<br>Day<br>Delinquent<br>Debt<br>Referral<br>Compliance<br>Report | 120<br>Day<br>Delinquent<br>Debt<br>Referral<br>Compliance<br>Report | /v2/debt/tror/data_act_compliance | The<br>120<br>Day<br>Delinquent<br>Debt<br>Referral<br>Compliance<br>Report<br>provides<br>access<br>to<br>tracking<br>and<br>benchmarking<br>compliance<br>with<br>the<br>requirements<br>of<br>a<br>key<br>provision<br>of<br>the<br>Digital<br>Accountability<br>and<br>Transparency<br>Act<br>of<br>2014<br>(the<br>DATA<br>Act).<br>This<br>table<br>provides |

| Dataset                                                               | Table<br>Name        | Endpoint                            | Endpoint<br>Description<br>quick<br>insights<br>into<br>federal<br>agency<br>compliance<br>rates,<br>as<br>well<br>as<br>information<br>on<br>the<br>number<br>of<br>eligible<br>debts<br>and<br>debts<br>referred<br>or<br>not<br>referred<br>each<br>quarter,<br>beginning<br>in<br>Fiscal<br>Year<br>2016.                                                                                                                                                                                                                                              |
|-----------------------------------------------------------------------|----------------------|-------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Accrual<br>Savings<br>Bonds<br>Redemption<br>Tables<br>(Discontinued) | Redemption<br>Tables | /v2/accounting/od/redemption_tables | The<br>Redemption<br>Tables<br>dataset<br>contains<br>monthly tables<br>that<br>list<br>the<br>redemption<br>value,<br>interest<br>earned,<br>and<br>yield<br>of<br>accrual<br>savings<br>bonds<br>purchased<br>since<br>1941.<br>Each<br>monthly<br>report<br>lists<br>the<br>redemption<br>value<br>of<br>all<br>bonds<br>at<br>the<br>time<br>of<br>publication.<br>Investors<br>and<br>bond<br>owners<br>can<br>use<br>this<br>dataset<br>as<br>an<br>easy and<br>understandable<br>reference<br>to<br>know<br>the<br>redemption<br>value<br>of<br>the |

| Dataset                                                                                      | Table<br>Name                                                                                | Endpoint                                                   | Endpoint<br>Description<br>bonds<br>they<br>hold.                                                                                                                                                                                                                                                                                                          |
|----------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------|------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Advances<br>to<br>State<br>Unemployment<br>Funds<br>(Social<br>Security Act<br>Title<br>XII) | Advances<br>to<br>State<br>Unemployment<br>Funds<br>(Social<br>Security Act<br>Title<br>XII) | /v2/accounting/od/title_xii                                | Monthly<br>balances<br>for<br>securities<br>outstanding<br>and<br>principal<br>outstanding<br>for<br>State<br>and<br>Local<br>Government<br>Series<br>(SLGS)<br>securities.                                                                                                                                                                                |
| Average<br>Interest<br>Rates<br>on<br>U.S.<br>Treasury<br>Securities                         | Average<br>Interest<br>Rates<br>on<br>U.S.<br>Treasury<br>Securities                         | /v2/accounting/od/avg_interest_rates                       | Average<br>interest<br>rates<br>for<br>marketable<br>and<br>non<br>marketable<br>securities.                                                                                                                                                                                                                                                               |
| Daily Treasury<br>Statement<br>(DTS)                                                         | Operating<br>Cash<br>Balance                                                                 | /v1/accounting/dts/operating_cash_balance                  | This<br>table<br>represents<br>the<br>Treasury<br>General<br>Account<br>balance.<br>Additional<br>detail<br>on<br>changes<br>to<br>the<br>Treasury<br>General<br>Account<br>can<br>be<br>found<br>in<br>the<br>Deposits<br>and<br>Withdrawals<br>of<br>Operating<br>Cash<br>table.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million. |
| Daily Treasury<br>Statement<br>(DTS)                                                         | Deposits<br>and<br>Withdrawals<br>of<br>Operating<br>Cash                                    | /v1/accounting/dts/<br>deposits_withdrawals_operating_cash | This<br>table<br>represents<br>deposits<br>and<br>withdrawals                                                                                                                                                                                                                                                                                              |

| Dataset                              | Table<br>Name                                                             | Endpoint                                                              | Endpoint<br>Description                                                                                                                                                                                                                                       |
|--------------------------------------|---------------------------------------------------------------------------|-----------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|                                      |                                                                           |                                                                       | from<br>the<br>Treasury<br>General<br>Account.<br>A<br>summary of<br>changes<br>to<br>the                                                                                                                                                                     |
|                                      |                                                                           |                                                                       | Treasury<br>Menu<br>General                                                                                                                                                                                                                                   |
|                                      |                                                                           |                                                                       | Account<br>can<br>be<br>found<br>in<br>the<br>Operating<br>Cash<br>Balance<br>table.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million.                                                                                                  |
| Daily Treasury<br>Statement<br>(DTS) | Public<br>Debt<br>Transactions                                            | /v1/accounting/dts/public_debt_transactions                           | This<br>table<br>represents<br>the<br>issues<br>and<br>redemption<br>of<br>marketable<br>and<br>nonmarketable<br>securities.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million.                                                          |
| Daily Treasury<br>Statement<br>(DTS) | Adjustment<br>of<br>Public<br>Debt<br>Transactions<br>to<br>Cash<br>Basis | /v1/accounting/dts/<br>adjustment_public_debt_transactions_cash_basis | This<br>table<br>represents<br>cash<br>basis<br>adjustments<br>the<br>issues<br>and<br>redemptions<br>Treasury<br>securities<br>in<br>the<br>Public<br>Debt<br>Transactions<br>table.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million. |
| Daily Treasury<br>Statement<br>(DTS) | Debt<br>Subject<br>to<br>Limit                                            | /v1/accounting/dts/debt_subject_to_limit                              | This<br>table<br>represents<br>the<br>breakdown<br>of<br>total<br>public                                                                                                                                                                                      |

| Dataset                              | Table<br>Name                    | Endpoint                                      | Endpoint<br>Description                                                                                                                                                                                      |
|--------------------------------------|----------------------------------|-----------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|                                      |                                  |                                               | debt<br>outstanding<br>as<br>it<br>relates<br>to<br>the<br>statutory debt<br>limit.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million.                                                  |
| Daily Treasury<br>Statement<br>(DTS) | Inter-Agency<br>Tax<br>Transfers | /v1/accounting/dts/inter_agency_tax_transfers | This<br>table<br>represents<br>the<br>breakdown<br>of<br>inter-agency<br>tax<br>transfers<br>within<br>the<br>federal<br>government.<br>All<br>figures<br>are<br>rounded<br>to<br>the<br>nearest<br>million. |

Showing 1 - 10 rows of 179 rows

![](_page_8_Figure_4.jpeg)

## **Fields by Endpoint**

To discover what **fields are available within each endpoint,** check out the corresponding dataset's details page for dataset-specific API documentation, or refer to its data dictionary.

**Not sure which dataset you need?** Head over to our [Datasets](https://fiscaldata.treasury.gov/datasets/) page to search and filter for datasets by topic, dates available, file type, and more.

# **Fiscal Service Data Registry**

The data [registry](https://fiscal.treasury.gov/data-registry/index.html) contains information about definitions, authoritative sources, data types, formats, and uses of common data across the federal government.

# **Methods**

#### 9/16/25, 1:35 AM API Documentation | U.S. Treasury Fiscal Data

**All requests will be HTTP GET requests**. Our APIs accept the GET method, one of the most common HTTP methods. The GET method is used to request data only (not modify). Note that GET requests can be cached, remain in browser history, be bookmarked, and have length restrictions.

# **Parameters**

**Parameters** can be included in an API request by modifying the URL. This will specify the criteria to determine which records will be returned, as well as the order and format of the data returned. More information about each parameter can be found below.

**Available parameters** include:

- Fields
- Filters
- Sorting
- Format
- Pagination

## **Fields**

**Parameter:** fields=

**Definition:** The fields parameter allows you to select which field(s) should be included in the response.

**Accepts:** The fields= parameter accepts a comma-separated list of field names.

**Required:** No, specifying fields is not required to make an API request.

**Default:** If desired fields are not specified, all fields will be returned.

**Notes:** When a file name passed to the fields parameter is not available for the endpoint accessed, an error will occur. Note that omitting fields can result in automatically aggregated and summed data results. For more information, view the full [documentation](#page-15-0) on Aggregation and Sums.

#### **Examples:**

Only return the following fields from a dataset: country\_currency\_desc, exchange\_rate, and record\_date.

#### ?fields=country\_currency\_desc,exchange\_rate,record\_date

Return the following fields from the Treasury Reporting Rates of Exchange dataset: country\_currency\_desc, exchange\_rate, and record\_date.

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/accou nting/od/rates\_of\_exchange?fields=country\_currency\_desc,exchange\_rate,re cord\_date

#### **Data Types**

All fields in a response will be **treated as strings** and enclosed in quotation marks (e.g., "field\_name"). The data type listed in a dataset's data dictionary or Fields table in dataset-specific API documentation indicates what the field is meant to be (e.g., date). **Note: This includes null values,** which will appear as strings ("null") rather than a blank or system-recognized null value. This allows you to **convert it to that data type in your language of choice.** For example, the Pandas library for Python helps you convert strings to 'datetime objects' and R allows you to convert characters to date objects using as.Date.

#### **Fields by Endpoint**

To discover what **fields are available within each endpoint,** check out the corresponding dataset's detail page for dataset-specific API documentation or refer to its data dictionary.

**Looking for field names for a specific dataset?** Jump to the Endpoints [by Dataset](#page-4-0) section to find your dataset of interest. Select any dataset name to view that dataset's details, including metadata, data dictionary, a preview table, graphs, and more!

**Not sure which dataset you need?** Head over to our [Datasets](https://fiscaldata.treasury.gov/datasets/) page to search and filter for datasets by topic, dates available, file type, and more.

### **Filters**

**Parameter:** filter=

**Definition:** Filters are used to view a subset of the data based on specific criteria. For example, you may want to find data that falls within a certain date range, or only show records which contain a value larger than a certain threshold.

**Accepts:** The filter parameter filter= accepts filters from the list below, as well as specified filter criteria. Use a colon at the end of a filter parameter to pass a value or list of values. For lists passed as filter criteria, use a comma-separated list within parentheses. Filter for specific dates using the format YYYY-MM-DD . To filter by multiple fields in a single request, do not repeat a filter call. Instead, apply an additional field to include in the filter separated by a comma, as shown in the following template:

#### &filter=field:prm:value,field:prm:value

**Required:** No, filters are not required to make an API request.

**Default:** When no filters are provided, the default response **will return all fields and all data.**

The filter parameter **accepts the following filters:**

- lt= Less than
- lte= Less than or equal to
- gt= Greater than
- gte= Greater than or equal to
- eq= Equal to
- in= Contained in a given set

**Examples:**

Return data if the fiscal year falls between 2007-2010.

#### ?filter=reporting\_fiscal\_year:in:(2007,2008,2009,2010)

Return data if the funding type ID is 202.

#### ?filter=funding\_type\_id:eq:202

From the Treasury Reporting Rates of Exchange dataset,

- only return specific fields (country\_currency\_desc, exchange\_rate, record\_date),
- only return data on the Canadian Dollar and Mexican Peso, and
- only return data that falls between January 1, 2020 and the present.

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/accou nting/od/rates\_of\_exchange?fields=country\_currency\_desc,exchange\_rate,re cord\_date&filter=country\_currency\_desc:in:(Canada-Dollar,Mexico-Peso),re cord\_date:gte:2020-01-01

## **Sorting**

**Parameter:** sort=

**Definition:** The sort parameter allows a user to sort a field in ascending (least to greatest) or descending (greatest to least) order.

**Accepts:** The sort parameter sort= accepts a comma-separated list of field names.

**Required:** No, sorting is not required to make an API request.

**Default:** When no sort parameter is specified, the default is to sort by the first column listed. Most API endpoints are thus sorted by date in ascending order (historical to most current).

**Notes:** You can nest sorting by passing the sort= parameter a comma-separated list.

**Examples:**

Sort the records returned by date in descending order, i.e. starting with the most recent date. ?sort= record\_date

Sort the Treasury Report on Receivables dataset by the Funding Type ID field in ascending order i.e., least to greatest.

```
https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v2/debt/
tror?sort=funding_type_id
```

Nested sorting (year, then month).

<span id="page-12-0"></span>https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v2/accou nting/od/debt\_to\_penny?fields=record\_calendar\_year,record\_calendar\_month &sort=-record\_calendar\_year,-record\_calendar\_month

### **Format**

**Parameter:** format=

**Definition:** The format parameter allows a user to define the output method of the response (CSV, JSON, XML).

**Accepts:** The format= parameter accepts xml , json , or csv as an input.

**Required:** No, format is not required to make an API request.

**Default:** When no format is specified, the default response format is JSON.

**Example:**

Return all data from the Debt to the Penny dataset in JSON format.

#### https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v2/accou nting/od/debt\_to\_penny?format=json

### **Pagination**

#### **Parameter:** page[size]= and page[number]=

**Definition:** The page size will set the number of rows that are returned on a request, and page number will set the index for the pagination, starting at 1. This allows the user to paginate through the records returned from an API request.

```
Accepts: The page[number]= and page[size]= parameters both accept integers.
```

**Required:** No, neither pagination parameters are required to make an API request.

**Default:** When no sort parameter is specified, the default is to sort by the first column listed. As a result, most API endpoints are sorted by date in ascending order (historical to most current).

**Notes:** When no page number or page size parameter is specified, the default response is

- Page number: 1
- Page size: 100

#### **Example:**

From the Treasury Offset Program dataset, return data with 50 records per page, and return the 10th page of data.

```
https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/debt/
top/top_state?page[number]=10&page[size]=50
```

# **Responses and Response Objects**

The response will be formatted according to the format input parameter specified in the [Format](#page-12-0) section and will be json, xml or csv. When format is not specified, the default response will be JSON. The response will be utf-8 and will have gzip support.

## **Response Codes**

The following response codes may be returned:

| Response<br>Code | Description                                                                                     |
|------------------|-------------------------------------------------------------------------------------------------|
| 200              | OK<br>-<br>Response<br>to<br>a<br>successful<br>GET<br>request                                  |
| 304              | Not<br>modified<br>-<br>Cached<br>response                                                      |
| 400              | Bad<br>Request<br>-<br>Request<br>was<br>malformed                                              |
| 403              | Forbidden<br>-<br>API<br>Key is<br>not<br>valid                                                 |
| 404              | Not<br>Found<br>-<br>When<br>a<br>non-existent<br>resource<br>is<br>requested                   |
| 405              | Method<br>Not<br>Allowed<br>-<br>Attempting<br>anything<br>other<br>than<br>a<br>GET<br>request |
| 429              | Too<br>Many Requests<br>-<br>Request<br>failed<br>due<br>to<br>rate<br>limiting                 |
| 500              | Internal<br>Server<br>Error<br>-<br>The<br>server<br>failed<br>to<br>fulfill<br>a<br>request    |

## **Meta Object**

The meta object provides metadata about the resulting payload from your API request. The object will contain the following:

- count: Record count for the response.
- labels: Mapping from result field to logical field names.
- dataTypes: Data type for each returned field.
- dataFormats: Size or format for each returned field.
- total-count: Total number of rows available in the dataset.
- total-pages: Total number of pages of data available based on the page size in the meta count response.

**Example Meta Object:**

```
"meta": {
 "count": 3790,
 "labels": {
 "country_currency_desc": "Country - Currency Description",
 "exchange_rate": "Exchange Rate",
 "record_date": "Record Date"
 },
 "dataTypes": {
 "country_currency_desc": "STRING",
 "exchange_rate": "NUMBER",
 "record_date": "DATE"
 },
 "dataFormats": {
 "country_currency_desc": "String",
 "exchange_rate": "10.2",
 "record_date": "YYYY-MM-DD"
 },
 "total-count": 3790,
 "total-pages": 1
}
```

## **Links Object**

The links object is an API argument to access the current (self), first, previous, next, and last page of data. It is suitable for creating URLs under user interface elements such as pagination buttons.

**Example Links Object:**

```
"links": {
 "self": "&page%5Bnumber%5D=1&page%5Bsize%5D=-1",
 "first": "&page%5Bnumber%5D=1&page%5Bsize%5D=-1",
 "prev": null,
 "next": null,
 "last": "&page%5Bnumber%5D=1&page%5Bsize%5D=-1"
}
```

## **Data Object**

The data object is the section of the response where the requested data will be returned. The other objects (e.g., meta object, links object) are sent to enable use of the requested data.

The data object beings with {"data":

### **Error Object**

If something goes wrong while creating the API response, an error object will be returned to the user. The error object will contain the following information:

- Error: The error name.
- Message: A detailed explanation of why the error occurred and how to resolve it.

**Example Error Object:**

```
{
 "error": "Invalid Query Param",
 "message": "Invalid query parameter 'sorts' with value '[-
record_date]'. For more information please see the documentation."
}
```

### **Pagination Header**

The pagination header will contain the link: header and allows a user to navigate pagination using just the APIs.

```
Link <url first> ; rel="first", <url prev> ; rel="prev"; <url next> ; rel="next"; <url last> ;
rel="last"
```

# **Aggregation and Sums**

In some cases, using a field list that excludes some of an endpoint's available fields will trigger automatic aggregation of non-unique rows and summing of their numeric values, etc. You should use this when searching for the sum total of a specific field.

For example, the API call for the sum total of the opening monthly balance within the Daily Treasury Statement dataset would read as:

#### https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/accou nting/dts/deposits\_withdrawals\_operating\_cash?fields=record\_date,transac tion\_today\_amt

Running this API call will yield a sum of all the totals in the selected field. In this case, the call yields the total sum of all opening monthly balances over the course of all dates available in the dataset.

# **Examples and Code Snippets**

## **Fields**

For the Treasury Reporting Rates of Exchange dataset,

Return only the following fields: country\_currency\_desc , exchange\_rate , and record\_date

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/acco unting/od/rates\_of\_exchange? fields=country\_currency\_desc,exchange\_rate,record\_date

## **Filters**

For the Treasury Reporting Rates of Exchange dataset,

- return the following fields: country\_currency\_desc , exchange\_rate , and record\_date
- return data only for the Canadian Dollar and Mexican Peso
- return data only if the date is on or after January 1, 2020.

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v1/acco unting/od/rates\_of\_exchange?fields=country\_currency\_desc,exchange\_rate, record\_date&filter=country\_currency\_desc:in:(Canada-Dollar,Mexico-Peso),record\_date:gte:2020-01-01

For the Debt to the Penny dataset,

- return the following fields: record\_calendar\_year , record\_calendar\_month
- return the most recent data first, i.e., return data sorted by year (descending order) and then month (descending order)

```
https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v2/acco
unting/od/debt_to_penny?
fields=record_calendar_year,record_calendar_month&sort=-
record_calendar_year,-record_calendar_month
```

### **Format**

For the Debt to the Penny dataset,

- return all the data
- return the data in JSON format

https://api.fiscaldata.treasury.gov/services/api/fiscal\_service/v2/acco unting/od/debt\_to\_penny?format=json

## **Pagination**

For the Treasury Offset Program dataset,

return the data on the 10th page, and each page returns 50 records of data

```
https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/debt
/top/top_state?page[number]=10&page[size]=50
```

## **Aggregation**

For the Daily Treasury Statement dataset,

Return the sum of all transactions today amounts for each transaction type on each record date

```
https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/acco
unting/dts/deposits_withdrawals_operating_cash?
fields=record_date,transaction_type,transaction_today_amt
```

## **Multi-dimension Datasets**

Many Fiscal Data datasets contain multiple tables or APIs, which relate to each other. Please see the Data Dictionary, Data Tables, Metadata, and Notes & Known Limitations tabs within the dataset properties section of each dataset page for more information.

**Table of Contents**

**Help**

[FAQ](https://fiscaldata.treasury.gov/about-us/#faq)

[Contact Us](mailto:fiscaldata@fiscal.treasury.gov?subject=Contact%20Us)

#### [Community](https://onevoicecrm.my.site.com/FiscalDataCommunity/s/) Site

**About Us**

[About Fiscal](https://fiscaldata.treasury.gov/about-us/#about-fiscal-data) Data

Release [Calendar](https://fiscaldata.treasury.gov/release-calendar/)

[Subscribe](https://fiscaldata.treasury.gov/about-us/#subscribe) to Our Mailing List

**Our Sites**

[USAspending](https://www.usaspending.gov/)

© 2025 Data Transparency [Accessibility](https://fiscal.treasury.gov/accessibility.html) [Privacy](https://fiscal.treasury.gov/privacy.html) Policy Freedom of [Information](https://fiscal.treasury.gov/foia.html) Act