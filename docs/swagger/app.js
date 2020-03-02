/* jshint esversion: 6 */

const express = require('express');
const app = express();
const swaggerUi = require('swagger-ui-express');
const YAML = require('yamljs');
const swaggerDocument = YAML.load('../api/ds-io-modality-swagger.yaml');
 
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));

app.listen(8081, "127.0.0.1");
console.log('Server running. Swagger docs available at http://127.0.0.1:8081/api-docs');