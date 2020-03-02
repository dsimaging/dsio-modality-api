# Dentsply Sirona Intraoral Imaging Modality
This repository contains specifications and documentation for the Intraoral Imaging Modality APIs and SDKs.

## Structure

* api - contains the OpenAPI spec for the API
* sdk - contains client side SDKs
* docs - contains documentation and node project for generating docs
* examples - contains example programs

## Building documentation
NodeJS programs to generate documentation are in the docs folder. Switch to the ./docs folder before executing any node or npm commands.

### ReDoc
Build the ReDoc static HTML by executing the following:
    
`npm run build-redoc`

The result will be an updated html file under the ./docs/redoc folder

### Swagger
Use the swagger-ui and express-js to locally serve the documentation using the swagger ui.

`npm run swagger`

The console will inform you when the server is running. Open a browser and navigate to the url indicated.
