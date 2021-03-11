# Dentsply Sirona Intraoral Imaging Modality
This repository contains specifications and documentation for the Intraoral Imaging Modality API.

## Structure

* api - contains the OpenAPI spec for the API
* docs - contains documentation and code for generating documentation
* sdk - contains client SDKs and sample applications

## API Specs
The api folder contains the OpenAPI spec in yaml format. This file can be used with Swagger Hub or other swagger tools for vieweing or editing the API.

Additionally, this folder contains a Postman collection in Json format. You can import this file into Postman and it will create a set of commands for each API in the spec. It is very useful to quickly configure Postman for testing the API.

## Documentation
NodeJS programs to generate documentation are in the docs folder. Switch to the ./docs folder before executing any node or npm commands.

### ReDoc
Build the ReDoc static HTML by executing the following:
    
`npm run build-redoc`

The result will be an updated html file under the ./docs/redoc folder. This file can be opened
with a browser to view the API documentation.

### Swagger
Use the swagger-ui and express-js to locally serve the documentation using the swagger ui.

`npm run swagger`

The console will inform you when the server is running. Open a browser and navigate to the url indicated. This provides an interactive Swagger UI documentation. It can be used to directly interact with a running instance of an IO Modality Service.

## SDK
The SDK folder contains language specific libraries and sample code. The SDK is not required for using the API, but they may be helpful in getting started.
