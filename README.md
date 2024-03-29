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
To get started with learning how to use the API, we suggest that you consult the [Wiki](https://github.com/dsimaging/dsio-modality-api/wiki) pages of this project. There you will find an overview and detailed information about the API. Other sources of API documentation can be found in the docs folder.

NodeJS programs to generate documentation are in the docs folder. Switch to the ./docs folder before executing any node or npm commands. Be sure to install local node modules required for the NodeJS programs by executing the following:

`npm install`

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
The SDK folder contains language specific libraries and sample code. The SDK is not required for using the API, but they may be helpful in getting started. Build scripts are provided for building the SDK.

We recommend that you build the SDK and samples to familiarize yourself with the SDK and its usage. When you are ready to distribute your product, you may consider using pre-built libraries or packages that are generated by this repository upon release. To download pre-built SDK libraries and packages, please refer to the [Releases](https://github.com/dsimaging/dsio-modality-api/releases) page.
