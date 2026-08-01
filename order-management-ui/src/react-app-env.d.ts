/// <reference types="react-scripts" />

// ✅ Declare CSS modules for TypeScript
declare module '*.css' {
  const content: { [className: string]: string };
  export default content;
}

// ✅ Declare CSS files for side-effect imports
declare module '*.css' {
  const content: string;
  export default content;
}

// ✅ Specifically for Bootstrap
declare module 'bootstrap/dist/css/bootstrap.min.css' {
  const content: string;
  export default content;
}

// ✅ For any other CSS files
declare module '*.module.css' {
  const classes: { [key: string]: string };
  export default classes;
}