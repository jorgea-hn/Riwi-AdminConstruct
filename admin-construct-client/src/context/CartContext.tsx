import React, { createContext, useContext, useState, useEffect } from 'react';

interface CartItem {
  id: string | number;
  name: string;
  price: number;
  quantity: number;
  type: 'product' | 'rental';
  startDate?: string;
  endDate?: string;
}

interface CartContextType {
  cartItems: CartItem[];
  addToCart: (item: CartItem) => void;
  removeFromCart: (index: number) => void;
  updateQuantity: (index: number, quantity: number) => void;
  clearCart: () => void;
  cartCount: number;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const CartProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  useEffect(() => {
    console.log('[CartContext] Initializing, loading from localStorage');
    const savedCart = JSON.parse(localStorage.getItem('cart') || '[]');
    console.log('[CartContext] Loaded cart items:', savedCart.length);
    setCartItems(savedCart);
  }, []);

  useEffect(() => {
    console.log('[CartContext] Cart updated, saving to localStorage. Items:', cartItems.length);
    localStorage.setItem('cart', JSON.stringify(cartItems));
  }, [cartItems]);

  const addToCart = (item: CartItem) => {
    console.log('[CartContext] Adding item to cart:', item.name, 'Type:', item.type);
    setCartItems((prev) => {
      // Check if item already exists (only for products, rentals are always unique due to dates)
      if (item.type === 'product') {
        const existingIndex = prev.findIndex((i) => i.id === item.id && i.type === 'product');
        if (existingIndex >= 0) {
          console.log('[CartContext] Product exists, updating quantity');
          const newCart = [...prev];
          newCart[existingIndex].quantity += item.quantity;
          return newCart;
        }
      }
      console.log('[CartContext] Adding new item to cart');
      return [...prev, item];
    });
  };

  const removeFromCart = (index: number) => {
    console.log('[CartContext] Removing item at index:', index);
    setCartItems((prev) => prev.filter((_, i) => i !== index));
  };

  const updateQuantity = (index: number, quantity: number) => {
    if (quantity < 1) return;
    console.log('[CartContext] Updating quantity at index:', index, 'New quantity:', quantity);
    setCartItems((prev) => {
      const newCart = [...prev];
      newCart[index].quantity = quantity;
      return newCart;
    });
  };

  const clearCart = () => {
    console.log('[CartContext] Clearing cart');
    setCartItems([]);
  };

  const cartCount = cartItems.reduce((total, item) => total + item.quantity, 0);

  return (
    <CartContext.Provider value={{ cartItems, addToCart, removeFromCart, updateQuantity, clearCart, cartCount }}>
      {children}
    </CartContext.Provider>
  );
};

export const useCart = () => {
  const context = useContext(CartContext);
  if (context === undefined) {
    throw new Error('useCart must be used within a CartProvider');
  }
  return context;
};
