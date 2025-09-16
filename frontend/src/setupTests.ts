import "@testing-library/jest-dom";

// Polyfills for Node.js environment
const { TextEncoder, TextDecoder } = require("util");

Object.assign(globalThis, {
  TextDecoder,
  TextEncoder,
});

// Mock IntersectionObserver
Object.assign(globalThis, {
  IntersectionObserver: class IntersectionObserver {
    constructor() {}
    disconnect() {}
    observe() {}
    unobserve() {}
    root = null;
    rootMargin = "";
    thresholds = [];
    takeRecords = () => [];
  },
});

// Mock ResizeObserver
Object.assign(globalThis, {
  ResizeObserver: class ResizeObserver {
    constructor() {}
    disconnect() {}
    observe() {}
    unobserve() {}
  },
});

// Mock window.matchMedia
Object.defineProperty(window, "matchMedia", {
  writable: true,
  value: (query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: () => {}, // deprecated
    removeListener: () => {}, // deprecated
    addEventListener: () => {},
    removeEventListener: () => {},
    dispatchEvent: () => {},
  }),
});

// Mock scrollTo
Object.assign(window, { scrollTo: () => {} });
