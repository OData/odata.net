import React from 'react';
import SwaggerUIReact from 'swagger-ui-react';

interface Props {
  swaggerJson: string | object | null;
}

const SwaggerUI: React.FC<Props> = ({ swaggerJson }) => {
  // Check if swaggerJson is null or undefined
  if (swaggerJson == null) {
    // Handle the case where swaggerJson is null or undefined
    return <div>Swagger JSON is missing or invalid.</div>;
  }

  return <SwaggerUIReact spec={swaggerJson} />;
};

export default SwaggerUI;
