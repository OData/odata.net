// SwaggerUI.tsx

import React, { useEffect, useState } from 'react';
import SwaggerUIReact from 'swagger-ui-react';
import 'swagger-ui-react/swagger-ui.css';

interface Props {
  swaggerJson: Record<string, any> | null;
}

const SwaggerUI: React.FC<Props> = ({ swaggerJson }) => {
  return <SwaggerUIReact spec={swaggerJson} />;
};

export default SwaggerUI;
